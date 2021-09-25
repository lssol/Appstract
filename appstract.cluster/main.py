#%%
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
from sklearn.cluster import DBSCAN, KMeans, AgglomerativeClustering
from sklearn import metrics
from sklearn.decomposition import PCA
from sklearn.model_selection import GridSearchCV, cross_val_predict, train_test_split
from importlib import reload
from sklearn.base import TransformerMixin
import time
from tqdm import tqdm
from pprint import pprint
from statistics import mean
import math

import importing
reload(importing)

import dom_structure
reload(dom_structure)

import preproc
reload(preproc)

import saving
reload(saving)

MAX_NB_PAGES = 1000

PARAMS = {
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
      # 'cluster_algo': 'kmeans'
      'cluster_algo': 'kmeans'
}
LABEL = 'kmeans_tfidf_branches_uneven_pca'

def concatenate_feature_vectors(feature_vectors):
      nb_docs          = len(list(feature_vectors.values())[0])
      total_vector_len = sum([vector.shape[1] for _, vector in feature_vectors.items()])
      
      X = np.zeros((nb_docs, total_vector_len))

      for i in range(0, nb_docs):
            X[i, :] = np.concatenate([feature_vectors['branches'][i,:], feature_vectors['classes'][i,:], feature_vectors['ids'][i,:]])

      pca = PCA(PARAMS['pca_final'])
      X = pca.fit_transform(X)
      print('[final] PCA Variance: ', pca.explained_variance_ratio_)
      plt.title('Projection of final vectors')
      plt.scatter(X[:, 0], X[:, 1])
      plt.show()
      
      return X
      
      
def find_best_clusterer(X):
      MAX_CLUSTERS = 40
      max_clusters = min(X.shape[0] - 1, MAX_CLUSTERS)

      clusterer_types = {
            'hierarchical': [],
            'kmeans': []
      }

      for k in range(1, max_clusters):
            clusterer_types['hierarchical'].append(AgglomerativeClustering(k))
            clusterer_types['kmeans'].append(KMeans(k))

      clusterers = clusterer_types[PARAMS['cluster_algo']]
      silhouettes = []
      y_preds = []
      for clusterer in clusterers:
            y_pred = clusterer.fit_predict(X)

            if y_pred.max() <= 1:
                  silhouettes.append(0)
                  continue
                  
            silhouettes.append(metrics.silhouette_score(X, y_pred))
            y_preds.append(y_pred)

      plt.figure()
      plt.plot(silhouettes)
      plt.title('Silhouettes for test and train for different values of k')
      plt.show()

      best_k = np.array(silhouettes).argmax()

      print('Best nb of clusters k = ', best_k)

      return y_preds[best_k]


def cluster_pages(domain):
      urls, indices, pages = importer.get_data(MAX_NB_PAGES, domain)
      nb_docs = len(urls)

      features = {}
      for page in tqdm(pages):
            for feature_name, feature in dom_structure.get_features(page).items():
                  features.setdefault(feature_name, []).append(feature)
      
      vectors = {}
      for feature_name, feature in features.items():
            preprocessor = preproc.get_preprocessor(feature_name, PARAMS['pca_features'][feature_name], PARAMS['tfidf'][feature_name], PARAMS['log'][feature_name])
            vectors[feature_name] = preprocessor.fit_transform(feature)

            print('[{}] PCA Variance: {}'.format(feature_name, preprocessor.named_steps['pca'].explained_variance_ratio_))

      X = concatenate_feature_vectors(vectors)

      predicted_labels = find_best_clusterer(X)

      silhouette = metrics.silhouette_score(X, predicted_labels)
      silhouette_values = metrics.silhouette_samples(X, predicted_labels)

      print('Silhouette is: ', silhouette)
      return saving.format(LABEL, PARAMS, domain, urls, predicted_labels, silhouette, silhouette_values)


importer = importing.Importer()
saver = saving.Saver()

domains = importer.get_available_domains()
silhouettes_per_app = {}
for domain in domains:
      print('Clustering domain: ', domain['domain'])
      result = {}
      try:
            result = cluster_pages(domain['domain'])
      except:
            print('An error occured when trying to handle this domain')
            continue
      saver.save(result)
      silhouettes_per_app[domain['domain'].replace('.', '_')] = result['silhouette']

# %%
saver.save_all_results(LABEL, silhouettes_per_app)


# %%
silhouettes_per_app

# %%
