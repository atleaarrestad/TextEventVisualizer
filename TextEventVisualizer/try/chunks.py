import spacy

nlp = spacy.load("en_core_web_sm")
# add this to make chunks and apply nlp for each chunk
def process_chunk(chunk, all_results, object_counts, context):
    chunk = context + chunk
    
    doc = nlp(chunk)

    important_events = []
    for token in doc:
        if "VERB" in token.pos_:
            subject = ''
            object = ''
            for child in token.children:
                if 'nsubj' in child.dep_:
                    subject = child.text
                elif 'dobj' in child.dep_:
                    object = child.text
            if subject != "" and object != "":
                important_events.append(((token, subject, object)))
                object_counts[object] = object_counts.get(object, 0) + 1

    all_results.extend(important_events)
# here I trie to fix the problem couldnt
    context = chunk[-50:]

    return context

all_results = []
object_counts = {}
context = ""

with open("donald_trump.txt") as file:
    chunk_size = 200 # Here you can change the size of the chunks to understand the problems
    while True:
        chunk = file.read(chunk_size)
        if not chunk:
            break
        context = process_chunk(chunk, all_results, object_counts, context)

all_results.sort(key=lambda event: object_counts[event[2]], reverse=True)

print("----- Gathered Events with subject and object -----")
for event in all_results:
    print(f"{event[1]} {event[0].text} {event[2]} (Count: {object_counts[event[2]]})")

print("------------------------------------------\n\n")

max_count_object = max(object_counts, key=object_counts.get)
most_frequent_object_events = [event for event in all_results if event[2] == max_count_object]

print("----- Top events of the text -----")
for event in most_frequent_object_events:
    print(f"{event[1]} {event[0].text} {event[2]} (Count: {object_counts[event[2]]})")
