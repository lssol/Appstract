from typing import List
import math
from typing import List, Tuple
from appstract.types import Cluster, Page

def dumbClusterer(pages: List[Page]) -> List[Cluster]:
    halfPages = math.floor((len(pages)/2))
    cluster1 = [(p, 0.5) for p in pages[::halfPages]]
    cluster2 = [(p, 0.5) for p in pages[halfPages::]]

    return [cluster1, cluster2]
