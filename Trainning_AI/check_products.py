# Check loaded products
import sys
import codecs
if sys.platform == 'win32':
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')

from app.simple_vector_store import SimpleVectorStore

vs = SimpleVectorStore()
print(f'Total documents: {len(vs.documents)}')
print('\nProducts loaded:')
for i, m in enumerate(vs.metadatas, 1):
    print(f'  {i}. {m["fruit_name"]}')
