from typing import List, Dict
from appstract.data.repository import Repository
from appstract.data.diff import compute_tasks
from appstract.clusterers.dumbClusterer import dumbClusterer
from appstract.types import Clusterer, Cluster

clusterers: Dict[str, Clusterer] = {
    "Dumb_Clusterer": dumbClusterer,
}

def synchronize(repo: Repository):
    print("Starting Synchronization...")
    tasks = compute_tasks(clusterers.keys())
    print(f'{len(tasks)} clustering tasks to synchronize...')

    for appId, clusterer_name in tasks:
        clusterer = clusterers[clusterer_name]
        print("Getting pages from app ", appId)
        pages = repo.getPagesFromAppId(appId)
        print(f'Retrieved {len(pages)} pages')
        clusters = clusterer(pages)
        displayClusters(clusters)

def displayClusters(clusters: List[Cluster]):
    for i, cluster in enumerate(clusters):
        print(f'Cluster {i}')
        for page, confidence in cluster:
            print(f'[{confidence}] {page.url}')
