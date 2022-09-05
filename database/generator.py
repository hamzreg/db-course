from faker import Faker
from random import randint, shuffle, choice, uniform
from string import ascii_letters

import faker

from constants import *


def generate_bonus_cards():
    """
        Генерация таблицы
        бонусных карт.
    """

    faker = Faker()

    file = open(BONUS_CARDS_FILE, 'w')

    for _ in range(NUM_RECORDS):
        bonuses = randint(MIN_BONUSES, MAX_BONUSES)
        phone = faker.unique.numerify('%%%%%%%%%%%')

        record = '{0}|{1}\n'.format(bonuses, phone)
        file.write(record)

    file.close()


def generate_customers():
    """
        Генерация таблицы
        покупателей.
    """

    faker = Faker()

    file = open(CUSTOMERS_FILE, 'w')

    bonus_cards = list(range(1, NUM_RECORDS + 1))
    shuffle(bonus_cards)

    for bonus_card in bonus_cards:
        name = faker.first_name()
        surname = faker.last_name()

        record = '{0}|{1}|{2}\n'.format(name, surname, bonus_card)
        file.write(record)
    
    file.close()


def generate_purchases(purchases):
    """
        Генерация таблицы
        покупок.
    """

    file = open(PURCHASES_FILE, 'w')

    customers = list(range(1, NUM_RECORDS + 1))
    shuffle(customers)

    for purchase in purchases:
        customer = choice(customers)
        customers.remove(customer)
        
        record = '{0:.4}|{1}|{2}\n'.format(purchase['price'], ACTIVE, customer)
        file.write(record)

    customers = list(range(1, NUM_RECORDS + 1))
    shuffle(customers)

    for customer in customers:
        record = '{0}|{1}|{2}\n'.format(uniform(MIN_PRICE, MAX_PRICE), CANCELED, customer)
        file.write(record)

    file.close()


def generate_sales(wines):
    """
        Генерация таблицы
        продаж.
    """

    faker = Faker()

    file = open(SALES_FILE, 'w')

    purchases_ids = list(range(1, NUM_RECORDS + 1))
    purchases = []

    for purchase_id in purchases_ids:
        wine = choice(wines)

        supplier_wine_id = wine['id']
        purchase_price = wine['price']
        selling_price = purchase_price * (1 + wine['percent'] / 100.0)
        margin = selling_price - purchase_price
        costs = uniform(MIN_COSTS, MAX_COSTS)
        wine_number = randint(MIN_WINE_NUMBER, MAX_WINE_NUMBER)
        profit = (margin - costs) * wine_number

        date = faker.date().split('-')
        date = str(date[2] + '.' + date[1] + '.' + date[0])

        purchase = {}
        purchase['purchase_id'] = purchase_id
        purchase['price'] = selling_price * wine_number
        purchases.append(purchase)

        record = '{0:.4}|{1:.5}|{2:.4}|{3:.4}|{4:.5}|{5}|{6}|{7}|{8}\n'.format(
            purchase_price, selling_price, margin, costs,
            profit, wine_number, date, purchase_id, supplier_wine_id)

        file.write(record)

    file.close()
    return purchases


def generate_wines():
    """
        Генерация таблицы
        вин.
    """

    file = open(WINES_FILE, 'w')
    wines = {}

    for i in range(NUM_RECORDS):
        color = choice(COLORS)
        sugar = choice(SUGAR)
        volume = choice(VOLUMES)
        alcohol = uniform(MIN_ALCOHOL, MAX_ALCOHOL)
        aging = randint(MIN_AGING, MAX_AGING)
        number = randint(MIN_NUMBER, MAX_NUMBER)
        wines[i + 1] = number

        if color == 'red':
            kind = choice(RED_KINDS)
        elif color == 'white':
            kind = choice(WHITE_KINDS)
        elif color == 'rose':
            kind = choice(ROSE_KINDS)

        record = '{0}|{1}|{2}|{3}|{4:.3}|{5}|{6}\n'.format(kind,
                  color, sugar, volume, alcohol, aging, number)
        file.write(record)

    file.close()
    return wines


def generate_suppliers():
    """
       Генерация таблицы
       поставщиков.
    """

    faker = Faker()

    file = open(SUPPLIERS_FILE, 'w')

    for _ in range(NUM_RECORDS):
        name = faker.unique.company()
        country = choice(COUNTRIES)
        experience = uniform(MIN_EXPERIENCE, MAX_EXPERIENCE)
        license = choice([True, False])

        record = '{0}|{1}|{2:.3}|{3}\n'.format(name, country,
                                            experience, license)
        file.write(record)
    
    file.close()


def generate_supplier_wines(wines):
    """
        Генерация таблицы
        вин поставщиков.
    """

    file = open(SUPPLIER_WINES_FILE, 'w')

    supplier_wines = []

    for i in range(NUM_RECORDS):
        wine_id = i + 1
        suppliers = list(range(1, NUM_RECORDS + 1))
        shuffle(suppliers)

        for _ in range(wines[wine_id]):
            supplier_id = choice(suppliers)
            suppliers.remove(supplier_id)
            price = uniform(MIN_PRICE, MAX_PRICE)
            percent = randint(MIN_PERCENT, MAX_PERCENT)
            rating = uniform(MIN_RATING, MAX_RATING)

            record = '{0}|{1}|{2:.4}|{3}|{4:.3}\n'.format(supplier_id,
                      wine_id, price, percent, rating)
            file.write(record)

            wine = {}
            wine['id'] = i
            wine['price'] = price
            wine['percent'] = percent

            supplier_wines.append(wine)

    file.close()

    return supplier_wines


def generate_users():
    """
        Генерация таблицы 
        пользователей.
    """

    customers = [[i, 'customer'] for i in range(1, NUM_RECORDS + 1)]
    suppliers = [[i, 'supplier'] for i in range(1, NUM_RECORDS + 1)]
    users = customers + suppliers
    shuffle(users)

    faker = Faker()

    file = open(USERS_FILE, 'w')

    for user in users:
        profile = faker.simple_profile()
        login = profile['username']
        password = ''.join(choice(ascii_letters) 
                   for i in range(randint(MIN_PASSWORD_LEN, MAX_PASSWORD_LEN)))

        record = '{0}|{1}|{2}|{3}\n'.format(user[0], login,
                                            password, user[1])
        file.write(record)

    file.close()


if __name__ == "__main__":
    generate_bonus_cards()
    generate_customers()
    generate_suppliers()
    generate_users()
    wines = generate_wines()
    supplier_wines = generate_supplier_wines(wines)
    prices = generate_sales(supplier_wines)
    generate_purchases(prices)
