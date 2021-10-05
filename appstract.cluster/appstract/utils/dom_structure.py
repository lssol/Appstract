#%%
from lxml import etree
from io import StringIO, BytesIO

class Extractor:
    MAX_LENGTH_ATTR = 21
    def __init__(self, page):
        parser    = etree.HTMLParser(remove_blank_text=True, remove_comments=True)
        tree      = etree.parse(StringIO(page), parser)
        self.root = tree.getroot()
        self._traverse()
    
    def _traverse(self):
        self.branches = []
        self.classes  = []
        self.ids      = []

        def traverse(el, current_branch: str):
            if not isinstance(current_branch, str) or not isinstance(el.tag, str):
                return
            
            if el.get('id') and len(el.get('id')) < self.MAX_LENGTH_ATTR:
                self.ids.append(el.get('id'))
            if el.get('class'):
                classes = [c for c in el.get('class').split() if len(c) < self.MAX_LENGTH_ATTR]
                self.classes.extend(classes)

            branch = ((current_branch + '-') if current_branch else '') + el.tag
            if len(el) == 0:
                self.branches.append(branch)
                return
            for child in el:
                traverse(child, branch)

        traverse(self.root, '')

def get_features(page: str):
    ex = Extractor(page)
    return {
        'branches': ' '.join(ex.branches),
        'ids': ' '.join(ex.ids),
        'classes': ' '.join(ex.classes)
    }


#%%
page= '<html><body><p>sacha</p><p id="sacha"><a class="c1 c2">brisset</a></p></body></html>'
get_features(page)

# %%
