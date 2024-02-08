import sqlite3
from sentence_transformers import SentenceTransformer
from sklearn.cluster import AgglomerativeClustering
import spacy
import numpy as np

db_path = "C:/Users/Stig/Documents/GitHub/TextEventVisualizer/TextEventVisualizer/database.db"

embedder = SentenceTransformer("all-MiniLM-L6-v2")

nlp = spacy.load("en_core_web_sm")


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



corpus_embeddings = corpus_embeddings / np.linalg.norm(corpus_embeddings, axis=1, keepdims=True)

# Perform agglomerative clustering
clustering_model = AgglomerativeClustering(
    n_clusters=None, distance_threshold=1.5
)  # , affinity='cosine', linkage='average', distance_threshold=0.4)
clustering_model.fit(corpus_embeddings)
cluster_assignment = clustering_model.labels_


clustered_sentences = {}
for sentence_id, cluster_id in enumerate(cluster_assignment):
    if cluster_id not in clustered_sentences:
        clustered_sentences[cluster_id] = []

    clustered_sentences[cluster_id].append(sentences[sentence_id])

unique_clusters = np.unique(cluster_assignment)

for cluster_id in unique_clusters:
    # Count the occurrences of the current cluster label in the cluster_assignment array
    num_sentences_in_cluster = np.count_nonzero(cluster_assignment == cluster_id)
    print("Number of sentences in Cluster", cluster_id + 1, ":", num_sentences_in_cluster)

# Sort the clustered sentences
sorted_clusters = sorted(clustered_sentences.items(), key=lambda x: x[0])

print("Sorted clusters")
print("Starting to write to file")

with open("C:/Users/Stig/Documents/GitHub/TextEventVisualizer/Clustering_test/agglomerative.txt", "w", encoding = "utf-8") as output:
    output.write("")
    for i, (cluster_id, cluster) in enumerate(sorted_clusters):
        output.write("Cluster " + str(cluster_id + 1) + "\n")
        for sentence in cluster:
            output.write(sentence + "\n")
        output.write("\n")

print("Clustering done. Output written to agglomerative.txt")
