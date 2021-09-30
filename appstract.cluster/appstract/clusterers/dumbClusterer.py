from typing import List
import math
from typing import List, Tuple
from appstract.types import Cluster, ClusteringResult, Page

def dumbClusterer(pages: List[Page]) -> ClusteringResult:
    halfPages = math.floor((len(pages)/2))
    cluster1 = Cluster(0.5, [(p, 0.5) for p in pages[::halfPages]])
    cluster2 = Cluster(0.5, [(p, 0.5) for p in pages[halfPages::]])

    return ClusteringResult(0.5, [cluster1, cluster2])
