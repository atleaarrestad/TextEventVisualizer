import sqlite3
from sentence_transformers import SentenceTransformer
from sklearn.cluster import KMeans
import spacy

db_path = "C:/Users/Stig/Documents/GitHub/TextEventVisualizer/TextEventVisualizer/database.db"

embedder = SentenceTransformer("all-MiniLM-L6-v2")

nlp = spacy.load("en_core_web_sm")

# try: 
    # Establish a connection to the SQLite database
    # conn = sqlite3.connect(db_path)
    # print("Connected to the database")

    # cursor = conn.cursor()

#     cursor.execute('SELECT * FROM Articles LIMIT 1')
#     row = cursor.fetchone()
#     print("Sample query executed successfully:", row)

# except sqlite3.Error as e:
#     print("Error connecting to the database:", e)

conn = sqlite3.connect(db_path)
print("Connected to the database")

cursor = conn.cursor()


cursor.execute('SELECT Description FROM Articles WHERE Category = "WORLD NEWS" AND HasBeenScraped = "1"')
# Execute SQL query to fetch headlines where category is some_value
# cursor.execute('SELECT Headline FROM Articles WHERE Category = "WORLD NEWS"')
rows = cursor.fetchall()

# Process the fetched data
headlines = [row[0] for row in rows]
print("Number of headlines fetched:", len(headlines))

# Close the database connection
conn.close()

# Now you can use the fetched headlines for further processing



corpus = ' '.join(headlines)
corpus = corpus.replace('\u2015', '-')

doc = nlp(corpus)
sentences = [sent.text for sent in doc.sents]   


corpus_embeddings = embedder.encode(sentences)