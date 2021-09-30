from typing import Callable, Dict, List, Tuple
from dataclasses import dataclass

@dataclass
class Page:
    content: str
    url: str

@dataclass
class Cluster:
    silhouette: float
    pages: List[Tuple[Page, float]]

@dataclass
class ClusteringResult:
    silhouette: float
    clusters: List[Cluster]

@dataclass
class ClusteringResultDatabase:
    silhouette: float
    clusters: List[List[Dict]]
    pagesHash: str
    applicationId: str
    domain: str
    nb_clusters: int
    clusterer: str

Clusterer = Callable[[List[Page]], ClusteringResult]