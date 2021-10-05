from appstract.types import ClusteringResult, Page, Cluster
from typing import List, Tuple
import pandas as pd
import numpy as np
from sklearn.cluster import DBSCAN, KMeans, AgglomerativeClustering
from sklearn import metrics
from sklearn.decomposition import PCA
from sklearn.model_selection import GridSearchCV, cross_val_predict, train_test_split
from importlib import reload
from sklearn.base import TransformerMixin
import time
from pprint import pprint
from statistics import mean
import math
import appstract.utils.dom_structure as dom_structure
import appstract.utils.preproc as preproc

DEFAULT_PARAMS = {
      'pca_features': {
            'branches': 15,
            'ids': 10,
            'classes': 10
      },
      'pca_final': 6,
      'tfidf': {
            'branches': False,
            'ids': True,
            'classes': True
      },
      'log': {
            'branches': False,
            'ids': False,
            'classes': False
      },
      'cluster_algo': 'kmeans',
      'max_nb_page': 1000
}

def branch_clustering(params = DEFAULT_PARAMS):
      def run(pages: List[Page]) -> ClusteringResult:
            predicted_labels, silhouette, silhouette_values = cluster_pages(pages, params)
            clusters = format_cluster(pages, predicted_labels, silhouette_values)

            return ClusteringResult(silhouette, clusters)

      return run

def concatenate_feature_vectors(feature_vectors):
      nb_docs          = len(list(feature_vectors.values())[0])
      total_vector_len = sum([vector.shape[1] for _, vector in feature_vectors.items()])
      
      X = np.zeros((nb_docs, total_vector_len))

      for i in range(0, nb_docs):
            X[i, :] = np.concatenate([feature_vectors['branches'][i,:], feature_vectors['classes'][i,:], feature_vectors['ids'][i,:]])
      
      return X
      
def dimensionality_reduction(X, nb_features):
      pca = PCA(nb_features)
      X = pca.fit_transform(X)

      return X

def find_best_clusterer(X, clusteringAlgorithm = 'kmeans'):
      MAX_CLUSTERS = 40
      max_clusters = min(X.shape[0] - 1, MAX_CLUSTERS)

      clusterer_types = {
            'hierarchical': [],
            'kmeans': []
      }

      for k in range(1, max_clusters):
            clusterer_types['hierarchical'].append(AgglomerativeClustering(k))
            clusterer_types['kmeans'].append(KMeans(k))

      clusterers = clusterer_types[clusteringAlgorithm]
      silhouettes = []
      y_preds = []
      for clusterer in clusterers:
            y_pred = clusterer.fit_predict(X)

            if y_pred.max() <= 1:
                  silhouettes.append(0)
                  continue
                  
            silhouettes.append(metrics.silhouette_score(X, y_pred))
            y_preds.append(y_pred)

      # plt.figure()
      # plt.plot(silhouettes)
      # plt.title('Silhouettes for test and train for different values of k')
      # plt.show()

      best_k = np.array(silhouettes).argmax()

      print('Best nb of clusters k = ', best_k)

      return y_preds[best_k]

def cluster_pages(pages: List[Page], params):
      features = {}
      for page in pages:
            for feature_name, feature in dom_structure.get_features(page.content).items():
                  features.setdefault(feature_name, []).append(feature)
      
      vectors = {}
      for feature_name, feature in features.items():
            preprocessor = preproc.get_preprocessor(feature_name, params['pca_features'][feature_name], params['tfidf'][feature_name], params['log'][feature_name])
            vectors[feature_name] = preprocessor.fit_transform(feature)

      X = concatenate_feature_vectors(vectors)
      X = dimensionality_reduction(X, params['pca_final'])

      predicted_labels = find_best_clusterer(X, params['cluster_algo'])

      silhouette: float = metrics.silhouette_score(X, predicted_labels)
      silhouette_values = metrics.silhouette_samples(X, predicted_labels)

      print('Silhouette is: ', silhouette)

      return predicted_labels, silhouette, silhouette_values

def format_cluster(pages: List[Page], predicted_clusters, silhouette_values) -> List[Cluster]:
      nb_clusters = max(predicted_clusters)
      clusters: List[Cluster] = []
      for k in range(0, nb_clusters):
            silhouettes_values_for_cluster_k: List[float] = silhouette_values[predicted_clusters == k]
            silhouette = mean(silhouettes_values_for_cluster_k)

            pages_for_cluster_k = np.array(pages)[predicted_clusters == k]
            cluster_pages = list(zip(pages_for_cluster_k, silhouettes_values_for_cluster_k))
            cluster = Cluster(silhouette, cluster_pages)
            clusters.append(cluster)

      return clusters
