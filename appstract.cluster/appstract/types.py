from typing import Callable, Dict, List, Tuple
from dataclasses import dataclass

@dataclass
class Page:
    Content: str
    Url: str

@dataclass
class Cluster:
    Silhouette: float
    Pages: List[Tuple[Page, float]]

@dataclass
class ClusteringResult:
    Siohouette: float
    Clusters: List[Cluster]

@dataclass
class ClusteringResultDatabase:
    Silhouette: float
    Clusters: List[List[Dict]]
    PagesHash: str
    ApplicationId: str
    Domain: str
    NbClusters: int
    Clusterer: str
    Outdated: bool

Clusterer = Callable[[List[Page]], ClusteringResult]