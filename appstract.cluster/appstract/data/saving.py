#%%
from pymongo import MongoClient
from sklearn import metrics
from statistics import mean
import numpy as np
from typing import Dict

def format(clusterer, domain, urls, pageHashes, predicted_clusters, silhouette, silhouette_values, applicationId):
    nb_clusters = max(predicted_clusters)

    clusters = []
    for k in range(0, nb_clusters):
        cluster = {'pages': []}

        silhouettes_values_for_cluster_k = silhouette_values[predicted_clusters == k]
        cluster['silhouette'] = mean(silhouettes_values_for_cluster_k)

        urls_for_cluster_k = np.array(urls)[predicted_clusters == k]

        for url, sil in zip(urls_for_cluster_k, silhouettes_values_for_cluster_k):
            cluster['pages'].append({
                'silhouette': sil,
                'url': url
            })
        clusters.append(cluster)

    return {
        'clusterer': clusterer, 
        'k': len(clusters),
        'domain': domain,
        'applicationId': applicationId,
        'silhouette': silhouette,
        'clusters': clusters
    }


class Saver:
    def __init__(self):
        uri = 'mongodb://wehave_prod%40service:AX3ohnia@datalakestar.amarislab.com:27018/?authMechanism=PLAIN&ssl=true'
        self.client = MongoClient(uri)
        self.collection = self.client.Wehave.clustering_result_image
        self.short_collection = self.client.Wehave.clustering_result_short
    
    def save(self, result):
        res = self.collection.find_one({'label': result['label'], 'domain': result['domain']})
        if res:
            print('This domain/label already exists')
            return

        self.collection.insert_one(result)
    
    def save_all_results(self, label: str, silhouettes_per_websites: Dict):
        avg = mean(silhouettes_per_websites.values())
        self.short_collection.insert_one({
            'name': label,
            'average_silhouette': avg,
            'silhouettes': silhouettes_per_websites
        })

#%%
