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
print("Number of description fetched:", len(headlines))

# Close the database connection
conn.close()

# Now you can use the fetched headlines for further processing



corpus = ' '.join(headlines)
corpus = corpus.replace('\u2015', '-')

doc = nlp(corpus)
sentences = [sent.text for sent in doc.sents]   


corpus_embeddings = embedder.encode(sentences)



# Perform kmean clustering
num_clusters = 20
clustering_model = KMeans(n_clusters=num_clusters)
clustering_model.fit(corpus_embeddings)
cluster_assignment = clustering_model.labels_

for i in range(num_clusters):
    print("Number of sentences in Cluster", i + 1, ":", cluster_assignment.tolist().count(i))

clustered_sentences = {i: [] for i in range(num_clusters)}
for sentence_id, cluster_id in enumerate(cluster_assignment):
    clustered_sentences[cluster_id].append(sentences[sentence_id])

print("Starting to write to file")

with open("C:/Users/Stig/Documents/GitHub/TextEventVisualizer/Clustering_test/output.txt", "w", encoding = "utf-8") as output:
    output.write("")
    for i, cluster in clustered_sentences.items():
        output.write("Cluster " + str(i + 1) + "\n")
        for sentence in cluster:
            output.write(sentence + "\n")
        output.write("\n")    

print("Clustering done. Output written to output.txt")

# # for i, cluster in enumerate(clustered_sentences):
# for i, cluster in clustered_sentences.items():
#     print("Cluster ", i + 1)
#     for sentence in cluster:
#         # print(sentence)
#         print(sentence.encode('utf-8', 'replace').decode('utf-8', 'replace'))
#     print("")

