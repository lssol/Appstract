## Specificatoins of the Appstract system

### Training phase
User enters: domain
(debug) User checks that clustering works
(debug) User checks that appstraction works (data extraction)

### Prediction phase
User enters: whole page

System returns:
- id of associated template
- id of each element of the page on the template

1. This id is application wide:
Two items from the menu will have the same id on all pages that user can send

2. This id template wide:
Two buy buttons on two product page will have the same id

3. This id is semantically rich
Two product title from a product list on amazon will have one shared component of their id.
Ex. This element is the title of the 5th product
                                     |coordinate of the instance element 
                        |id in the template


# Flow

## Create an application
Provide domain name.

## Explore the domain
Start robot that collects pages from the domain (by random walk through links from pages)

## Clustering
1. Start clustering for app
2. Visualize result of clustering
We can have several types of clustering that have been applied to a given application.

## Appstraction
Take the centroids of all clusters and send them to the appstractor.

-> Returns a model of the application.
Save the model in the DB.

A model `M`: a list of templates.

When we encounter a new page `p` from an application `A`, we run:
- Get `appstract(p, M)`
  - Check which template from `M` correpsonds to `p`: `p_t`
    -> for (template in M) 
        tree-distance (p, template)
        -> take the closest template
  - Match the ids of `p` with the template `p_t`

## Script to do some data extraction

For each element in `p` -> which id it corresponds to in `M`

## Plugin that applies appstract to generate identified actions when a user browses
- A user installs the plugin
- The user browses

-> We get sequence of identified action

## Apply appstract to recordings from RRWEB

What we have: sequence of rrweb **events**. 
What we want: sequence of identified actions (identified in appstract)

## Study the sequence of identified actions (Dany)
Predict next action.

## Build a system to identify mutations

## Collect action/mutations

## Create associate action-mutation


# Demos

## Data extraction


## Bug detection
There are 2 types of bug:
1. Active bug:
I click on something, to open something else and it doesn't open.

2. Pasive bug:
I don't see the button.
The prices are wrong.

To show that we can do 1:
Browse on a page and introduce bugs. Show we can detect anomalies.

To show that we can do 2:
- Navigate
- Show next most likely clicks

## Detect behavior patterns (UX improvement)
- Random
- Browsing
- Goal oriented (ERP)

For the goal oriented ones.
-> Predictability = efficiency
-> Propose to measure the evolution of the efficiency on different UIs

Use ogame dataset.


