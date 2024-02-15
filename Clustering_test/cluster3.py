import sqlite3
import numpy as np
from sentence_transformers import SentenceTransformer
from sklearn.cluster import KMeans
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import normalize
import spacy

db_path = "C:/Users/Stig/Documents/GitHub/TextEventVisualizer/TextEventVisualizer/database.db"

embedder = SentenceTransformer("all-MiniLM-L6-v2")

nlp = spacy.load("en_core_web_sm")

conn = sqlite3.connect(db_path)
print("Connected to the database")

cursor = conn.cursor()


cursor.execute('SELECT Description FROM Articles WHERE Category = "WORLD NEWS" AND HasBeenScraped = "1"')
# Execute SQL query to fetch data for clustering
rows = cursor.fetchall()

# Process the fetched data
headlines = [row[0] for row in rows]
print("Number of description fetched:", len(headlines))

# Close the database connection
conn.close()



# further processing

corpus = ' '.join(headlines)
corpus = corpus.replace('\u2015', '-')

doc = nlp(corpus)
sentences = [sent.text for sent in doc.sents]   


corpus_embeddings = embedder.encode(sentences)

sample = {
    "covid": {"covid": 10, "coronavirus": 10, "covid-19": 10, "virus": 10, "patient": 10, "epidemic": 10, "pandemic": 10, "vaccine": 10, 
                "vaccination": 10, "mask": 10, "lockdown": 10, "quarantine": 10, "social distancing": 10, "infection": 10, "infectious": 10, 
                "World Health Organization": 10, "wuhan": 5},
    # "wildfire": {"wildfire": 5, "wildfires": 5},
    # "taiwan": {"taiwan": 5, "hong kong": 5, "china": 2, "taipei": 5, "beijing": 5},
    # "Donald Trump": {"Donald Trump": 5, "Donald": 5, "Trump": 5}
}


# Calculate TF-IDF weighted features
vectorizer = TfidfVectorizer()
X = vectorizer.fit_transform(sentences)
feature_names = vectorizer.get_feature_names_out()

# Apply custom weights
# weighted_X = X.copy()
# for word, weight in sample.items():
#     if word in feature_names:
#         indices = np.where(feature_names == word)[0]
#         for idx in indices:
#             weighted_X[:, idx] *= weight

weighted_X = X.copy()
combined_indices = []
for group_name, word_dict in sample.items():
    combined_group_values = np.zeros((len(sentences), 1))
    for word, weight in word_dict.items():
        if word in feature_names:
            idx = np.where(feature_names == word)[0][0]
            combined_indices.append(idx)
            weighted_X[:, idx] *= weight
            combined_group_values += weighted_X[:, idx].reshape(-1, 1)
    # Create a combined feature for the group
    for idx in combined_indices:
        weighted_X[:, idx] = 0  # Zero out individual feature values
    weighted_X[:, combined_indices[0]] = combined_group_values.flatten()  # Assign combined feature values


corpus_embeddings = normalize(weighted_X)


# Perform kmean clustering
num_clusters = 50
clustering_model = KMeans(n_clusters=num_clusters)
clustering_model.fit(corpus_embeddings)
cluster_assignment = clustering_model.labels_

for i in range(num_clusters):
    print("Number of sentences in Cluster", i + 1, ":", cluster_assignment.tolist().count(i))
print("Total numbers in clusters", len(cluster_assignment))    



clustered_sentences = {i: [] for i in range(num_clusters)}
for sentence_id, cluster_id in enumerate(cluster_assignment):
    clustered_sentences[cluster_id].append(sentences[sentence_id])

print("Starting to write to file")

with open("C:/Users/Stig/Documents/GitHub/TextEventVisualizer/Clustering_test/output3.txt", "w", encoding = "utf-8") as output:
    output.write("")
    for i, cluster in clustered_sentences.items():
        output.write("Cluster " + str(i + 1) + "\n")
        for sentence in cluster:
            output.write(sentence + "\n")
        output.write("\n")    

print("Clustering done. Output written to output3.txt")



