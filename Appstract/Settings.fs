module Appstract.Settings

open tree_matching_csharp
open System

let sftmParameters =
    SftmTreeMatcher.Parameters(
        NoMatchCost = 4.3,
        MaxPenalizationChildren        = 0.4,
        MaxPenalizationParentsChildren = 0.2,
        LimitNeighbors = 100000,
        MetropolisParameters = Metropolis.Parameters(
            Gamma                   = 1f, // MUST be < 1
            Lambda                  = 2.5f,
            NbIterations            = 1,
            MetropolisNormalisation = false                  
        ),
        MaxTokenAppearance = Func<int, int>(float >> sqrt >> int),
        PropagationParameters = SimilarityPropagation.Parameters(
            Envelop    = [|0.9; 0.1; 0.01|],
            Parent     = 0.4,
            Sibling    = 0.0,
            SiblingInv = 0.0,
            ParentInv  = 0.9,
            Children   = 0.0
        )
    )
    
let MAX_LCS = 100
let MIN_SIZE_CLUSTER = 2