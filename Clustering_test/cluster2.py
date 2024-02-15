import sqlite3
from sentence_transformers import SentenceTransformer
from sklearn.cluster import KMeans
import spacy
import numpy as np
from collections import defaultdict

db_path = "C:/Users/Stig/Documents/GitHub/TextEventVisualizer/TextEventVisualizer/database.db"

embedder = SentenceTransformer("all-MiniLM-L6-v2")

nlp = spacy.load("en_core_web_sm")

conn = sqlite3.connect(db_path)
print("Connected to the database")

cursor = conn.cursor()

cursor.execute('SELECT Description FROM Articles WHERE Category = "WORLD NEWS" AND HasBeenScraped = "1"')
rows = cursor.fetchall()

descriptions = [row[0] for row in rows]

print("Number of descriptions fetched:", len(descriptions))

conn.close()

# Named Entity Recognition (NER) to extract events
events = defaultdict(list)
for description in descriptions:
    doc = nlp(description)
    for ent in doc.ents:
        if ent.label_ == "EVENT":
            events[ent.text].append(description)

# Create corpus and calculate weights based on events
corpus = ' '.join(descriptions)
corpus = corpus.replace('\u2015', '-')
corpus_embeddings = embedder.encode(corpus)

# Define weights for sentences based on events
event_weights = np.zeros(len(corpus_embeddings))
for event, event_descriptions in events.items():
    event_embedding = embedder.encode(' '.join(event_descriptions))
    similarity_scores = np.zeros(len(corpus_embeddings))  # Initialize similarity scores for the event
    for idx, description in enumerate(event_descriptions):
        description_embedding = embedder.encode(description)
        similarity_scores += corpus_embeddings.dot(description_embedding.T)
    event_indices = [i for i, desc in enumerate(descriptions) if desc in event_descriptions]
    event_weights[event_indices] = max(similarity_scores)

# Perform k-means clustering with event-based weights
num_clusters = 20
clustering_model = KMeans(n_clusters=num_clusters)
clustering_model.fit(corpus_embeddings.reshape(-1, 1), sample_weight=event_weights)
cluster_assignment = clustering_model.labels_

if len(set(cluster_assignment)) < num_clusters:
    print("Warning: Number of distinct clusters found is smaller than n_clusters.")
    num_clusters = len(set(cluster_assignment))

# Print number of sentences in each cluster
for i in range(num_clusters):
    print("Number of sentences in Cluster", i + 1, ":", cluster_assignment.tolist().count(i))

# Assign sentences to clusters
clustered_sentences = {i: [] for i in range(num_clusters)}
for sentence_id, cluster_id in enumerate(cluster_assignment):
    if cluster_id < num_clusters:
        clustered_sentences[cluster_id].append(descriptions[sentence_id])
    else:
        print(f"Invalid cluster index {cluster_id} for sentence {sentence_id}")

# Write clustered sentences to file
print("Starting to write to file")

with open("C:/Users/Stig/Documents/GitHub/TextEventVisualizer/Clustering_test/output2.txt", "w", encoding="utf-8") as output:
    output.write("")
    for i, cluster in clustered_sentences.items():
        output.write("Cluster " + str(i + 1) + "\n")
        for sentence in cluster:
            output.write(sentence + "\n")
        output.write("\n")    

print("Clustering done. Output written to output2.txt")