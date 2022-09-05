NUM_RECORDS = 1000

# Файлы таблиц
BONUS_CARDS_FILE = 'data/bonusCards.csv'
CUSTOMERS_FILE = 'data/customers.csv'
PURCHASES_FILE = 'data/purchases.csv'
SALES_FILE = 'data/sales.csv'
WINES_FILE = 'data/wines.csv'
SUPPLIERS_FILE = 'data/suppliers.csv'
SUPPLIER_WINES_FILE = 'data/supplierWines.csv'
USERS_FILE = 'data/users.csv'

# Бонусные карты
MIN_BONUSES = 0
MAX_BONUSES = 1000

# Покупки
ACTIVE = 1
CANCELED = 0

# Вина
WHITE_KINDS = ['Albarino', 'Aligote', 'Arneis', 'Asti Spumante',
               'Auslese', 'Blanc de Blancs', 'Blanc de Noirs', 'Boal',
               'Cava', 'Champagne', 'Chardonnay', 'Chenin Blanc',
               'Colombard', 'Constantia', 'Cortese', 'Eiswein',
               'Frascati', 'Gewurztraminer', 'Grappa', 'Johannisberg Riesling',
               'Kir', 'Lambrusco', 'Liebfraumilch', 'Madeira',
               'Marc', 'Marsala', 'Marsanne', 'Moscato',
               'Muller-Thurgau', 'Muscat', 'Pinot Blanc', 'Pinot Gris',
               'Pinot Meunier', 'Retsina', 'Roussanne', 'Sauterns',
               'Sauvignon Blanc', 'Semillon', 'Soave', 'Tokay',
               'Traminer', 'Trebbiano', 'Ugni Blanc', 'Verdicchio', 
               'Viognier']

RED_KINDS = ['Amarone', 'Banylus', 'Barbaresco', 'Bardolino', 
             'Barolo', 'Beaujolais',  'Blanc de Noirs', 'Brunello',
             'Cabernet Franc', 'Cabernet Sauvignon', 'Cahors', 'Carmenere',
             'Chateauneuf-du-Pape',  'Chianti', 'Claret', 'Dolcetto',
             'Gamay', 'Gamay Beaujolais', 'Gattinara', 'Gewurztraminer',
             'Kir', 'Lambrusco', 'Malbec', 'Marc', 'Marsala', 
             'Merlot', 'Montepulciano', 'Nebbiolo', 'Petit Verdot', 
             'Petite Sirah', 'Pinot Meunier', 'Pinot Noir', 'Pinotage',
             'Port', 'Sangiovese', 'Sherry', 'Valpolicella', 'Zinfandel']

ROSE_KINDS = ['Blush', 'Cava', 'Champagne', 'Grenache',
             'Kir', 'Lambrusco', 'Marc']

COLORS = ['red', 'white', 'rose']

SUGAR = ["dry", "semi-dry", "semi-sweet", "sweet"]

VOLUMES = [0.1875, 0.75, 1.5, 3, 4.5, 6, 9, 12, 15, 18, 20,
           25, 27, 30]

MIN_ALCOHOL = 7.5
MAX_ALCOHOL = 22

MIN_AGING = 2
MAX_AGING = 10

MIN_NUMBER = 1
MAX_NUMBER = 5

MIN_RATING = 0
MAX_RATING = 10

# Поставщики
COUNTRIES = ['France', 'Italy', 'Spain', 'USA', 'Argentina',
             'Australia', 'China', 'South Africa', 'Germany',
             'Chile', 'Russia', 'Portugal', 'Romania', 'Greece',
             'Hungary', 'Brazil', 'Austria', 'Moldova', 'Bulgaria',
             'New Zealand', 'Croatia']

MIN_EXPERIENCE = 1
MAX_EXPERIENCE = 100

# Продажи
MIN_PRICE = 118
MAX_PRICE = 1000

MIN_PERCENT = 43
MAX_PERCENT = 100

MIN_COSTS = 10
MAX_COSTS = 50

MIN_WINE_NUMBER = 1
MAX_WINE_NUMBER = 5

# Пользователи
MIN_PASSWORD_LEN = 8
MAX_PASSWORD_LEN = 16