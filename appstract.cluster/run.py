import data.differ as differ
import clusterers.dumbClusterer as dumbClusterer

clusterers = {
    "Dumb_Clusterer": dumbClusterer,
    # "Branch_Clusterer": dumbClusterer,
}

def synchronize():
    differ.compute_tasks()

