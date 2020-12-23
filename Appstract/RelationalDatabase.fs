namespace Appstract.RelationalDatabase

type ColumnId = ColumnId of string
type TableId = TableId of string

type Cell =
    | Value of string
    | ForeignKey of Row
and Row = Map<ColumnId, Cell>

type Table = Table of Row seq

type RelDb = RelDb of Map<TableId, Table> with
    static member Empty = RelDb Map.empty
    
    member this.TableMap =
        let (RelDb db) = this
        db
        
    member this.GetOrCreateTable id =
        let table = this.TableMap.TryFind id
        match table with
        | Some t -> t
        | None -> Table Seq.empty


    
