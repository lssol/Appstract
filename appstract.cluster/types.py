from typing import Callable, List, Tuple
from dataclasses import dataclass

@dataclass
class Page:
    content: str
    url: str

Confidence = float
Cluster = List[Tuple[Page, Confidence]]
Clusterer = Callable[[List[Page]], List[Cluster]]