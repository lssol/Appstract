import data.differ as differ
import clusterers.dumbClusterer as dumbClusterer

clusterers = {
    "Dumb_Clusterer": dumbClusterer,
    # "Branch_Clusterer": dumbClusterer,
}

def synchronize():
    tasks = differ.compute_tasks(clusterers)
    for appId, clusterer in tasks:
        clusterer()



