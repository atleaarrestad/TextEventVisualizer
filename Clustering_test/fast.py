import sqlite3
from sentence_transformers import SentenceTransformer, util
import spacy
import time

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
print("Number of headlines fetched:", len(headlines))

# Close the database connection
conn.close()

# Now you can use the fetched headlines for further processing



corpus = ' '.join(headlines)
corpus = corpus.replace('\u2015', '-')

doc = nlp(corpus)
sentences = [sent.text for sent in doc.sents]   


corpus_sentences = list(sentences)
print("Encoding the corpus.")
corpus_embeddings = embedder.encode(corpus_sentences, batch_size=64, show_progress_bar=True, convert_to_tensor=True)

print("Start clustering")
start_time = time.time()

# Two parameters to tune:
# min_cluster_size: Only consider cluster that have at least 25 elements
# threshold: Consider sentence pairs with a cosine-similarity larger than threshold as similar
clusters = util.community_detection(corpus_embeddings, min_community_size=1, threshold=0.45)

print("Clustering done after {:.2f} sec".format(time.time() - start_time))

print("Starting to write to file")

with open("C:/Users/Stig/Documents/GitHub/TextEventVisualizer/Clustering_test/fast.txt", "w", encoding = "utf-8") as output:
    output.write("")
    for cluster_id, cluster in enumerate(clusters):
        output.write(f"Cluster {cluster_id + 1}:\n")
        for sentence_index in cluster:
            output.write(f"{corpus_sentences[sentence_index]}\n")
        output.write("\n")

    # for i, cluste in clustered_sentences.items():
    #     output.write("Cluster " + str(i + 1) + "\n")
    #     for sentence in cluster:
    #         output.write(sentence + "\n")
    #     output.write("\n")    

print("Clustering done. Output written to fast.txt")

