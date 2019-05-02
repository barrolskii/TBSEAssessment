open System
open System.IO
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Diagnostics
open System.Linq

// File path for reference
// @C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug

type Date = 
    struct
        val week: int
        val year: int

        new (initWeek, initYear) =
            { week = initWeek; year = initYear }
    end


type Store =
    struct
        val storeCode: string
        val storeLoc: string

        new (code, loc) =
            { storeCode = code; storeLoc = loc }
    end


type Order =
    struct
        val store: Store
        val date: Date
        val supplier: string
        val supplierType: string
        val cost: double

        new (store, date, supplier, supplierType, cost) =
            { store = store; date = date; supplier = supplier; supplierType = supplierType; cost = cost; }
    end

let stores = new Dictionary<string, Store>()
let dates = new ConcurrentQueue<Date>()
let orders = new ConcurrentQueue<Order>()

let PrintCommands() =
    printfn "===================================================================================\n"
    printfn "-h or -help for a list of all commands"
    printfn "-ps or -printstores to print all stores"
    printfn "-pst or -printsuppliertypes to print all supplier types"
    printfn "-psup or -printsuppliers to print all suppliers"
    printfn "-q or -quit to exit the application"
    printfn "\n==================================================================================="


let PrintSelectionOptions() =
    printfn "==================================================================================="
    printfn "1: Total cost of all orders"
    printfn "2: Cost of all orders for a single store"
    printfn "3: Cost of all orders in a week for all stores"
    printfn "4: Cost of all orders in a week for a single store"
    printfn "5: Cost of all orders to a supplier"
    printfn "6: Cost of all orders to a supplier type"
    printfn "7: Cost of orders in a week for a supplier type"
    printfn "8: Cost of orders to a supplier type for a store"
    printfn "9: Cost of orders in a week for a supplier type for a store"
    printfn "==================================================================================="


let PrintStores() =
    for store in stores do
        printfn "%s : %s" store.Key store.Value.storeLoc


let PrintSupplierTypes() =
    let supplierTypes = orders.AsParallel().Select(fun order -> order.supplierType).Distinct().OrderBy(fun order -> order)

    for supplierType in supplierTypes do
        printfn "%s" supplierType


let PrintSuppliers() =
    let suppliers = orders.AsParallel().Select(fun order -> order.supplier).Distinct().OrderBy(fun order -> order)

    for supplier in suppliers do
        printfn "%s" supplier


let TotalCostOfAllOrders() =
    let totalCost: double = orders.Sum(fun order -> order.cost)

    printfn "Total cost of all orders is: £%.2f" totalCost


let CostOfAllOrdersForASingleStore() =
    printfn "Please enter the store you would like to search"
    let store: string = Console.ReadLine()

    let totalCost: double = orders.Where(fun order -> order.store.storeCode = store).Select(fun order -> order.cost).Sum()
    printfn "Total cost of orders for %s is: £%.2f" store totalCost


let CostOfAllOrdersInAWeekForAllStores() =
    printfn "Please enter the week you want to search"
    let week: int = Convert.ToInt32(Console.ReadLine())

    let totalCost: double = orders.Where(fun order -> order.date.week = week).Select(fun order -> order.cost).Sum()
    printfn "Total cost of all orders for week %d: £%.2f" week totalCost


let CostOfAllOrdersInASingleWeekForAStore() =
    printfn "Please enter the week you want to search"
    let week: int = Convert.ToInt32(Console.ReadLine())

    printfn "Please enter the store you want to search"
    let store: string = Console.ReadLine()

    printfn "Enter the year you would like to search"
    let year: int = Convert.ToInt32(Console.ReadLine())

    let totalCost: double = orders.Where(fun order -> order.date.week = week && order.store.storeCode = store && order.date.year = year).Select(fun order -> order.cost).Sum()
    printfn "Total cost of all orders for %s in week %d: £%.2f" store week totalCost


let CostOfAllOrdersToASupplier() =
    printfn "Please enter the supplier you want to search"
    let supplier: string = Console.ReadLine()

    let totalCost: double = orders.Where(fun order -> order.supplier = supplier).Select(fun order -> order.cost).Sum()
    printfn "Total cost for %s: £%.2f" supplier totalCost


let CostOfAllOrdersToASupplierType() =
    printfn "Please enter the supplier type you want to search"
    let supplierType: string = Console.ReadLine()

    let totalCost: double = orders.Where(fun order -> order.supplierType = supplierType).Select(fun order -> order.cost).Sum()
    printfn "Total cost for %s: £%.2f" supplierType totalCost


let CostOfAllOrdersInAWeekToASupplierType() =
    printfn "Please enter the supplier type you want to search"
    let supplierType: string = Console.ReadLine()

    printfn "Please enter the week you want to search"
    let week: int = Convert.ToInt32(Console.ReadLine())

    printfn "Enter the year you would like to search"
    let year: int = Convert.ToInt32(Console.ReadLine())

    let totalCost: double = orders.Where(fun order -> order.supplierType = supplierType && order.date.week = week && order.date.year = year).Select(fun order -> order.cost).Sum()
    printfn "Total cost for %s in week %d: £%.2f" supplierType week totalCost


let CostOfAllOrdersToASupplierTypeForAStore() =
    printfn "Please enter the supplier type you want to search"
    let supplierType: string = Console.ReadLine()

    printfn "Please enter the store you would like to search"
    let store: string = Console.ReadLine()

    let totalCost: double = orders.Where(fun order -> order.store.storeCode = store && order.supplierType = supplierType).Select(fun order -> order.cost).Sum()
    printfn "Total cost for %s in store %s: £%.2f" supplierType store totalCost


let CostOfAllOrdersInAWeekToASupplierTypeForAStore() =
    printfn "Please enter the supplier type you want to search"
    let supplierType: string = Console.ReadLine()

    printfn "Please enter the store you would like to search"
    let store: string = Console.ReadLine()

    printfn "Please enter the week you want to search"
    let week: int = Convert.ToInt32(Console.ReadLine())

    printfn "Enter the year you would like to search"
    let year: int = Convert.ToInt32(Console.ReadLine())

    let totalCost: double = orders.Where(fun order -> order.supplierType = supplierType && order.store.storeCode = store && order.date.week = week && order.date.year = year)
                                   .Select(fun order -> order.cost)
                                   .Sum()

    printfn "Total cost for %s in store %s for week %d: £%.2f" supplierType store week totalCost


let HandleInput(input: string) =
    match input with
    | "1" -> TotalCostOfAllOrders()
    | "2" -> CostOfAllOrdersForASingleStore()
    | "3" -> CostOfAllOrdersInAWeekForAllStores()
    | "4" -> CostOfAllOrdersInASingleWeekForAStore()
    | "5" -> CostOfAllOrdersToASupplier()
    | "6" -> CostOfAllOrdersToASupplierType()
    | "7" -> CostOfAllOrdersInAWeekToASupplierType()
    | "8" -> CostOfAllOrdersToASupplierTypeForAStore()
    | "9" -> CostOfAllOrdersInAWeekToASupplierTypeForAStore()
    | _ -> printfn "Please select a valid option or enter q to quit"


let CheckInput(input: string) =
    match input with
    | "-h" -> PrintCommands()
    | "-help" -> PrintCommands()
    | "-ps" -> PrintStores()
    | "-printstores" -> PrintStores()
    | "-pst" -> PrintSupplierTypes()
    | "-printsuppliertypes" -> PrintSupplierTypes()
    | "-psup" -> PrintSuppliers()
    | "-printsuppliers" -> PrintSuppliers()
    | _ -> HandleInput(input)



let MainLoop() = 
    PrintSelectionOptions()

    let mutable quit = false
    while quit <> true do
        let input = Console.ReadLine()

        match input with
        | "-q" -> quit <- true
        | "-quit" -> quit <- true
        | _ -> CheckInput(input)


type Data() =
    member x.ReadAllData() =

        let folderPath : string = "StoreData"
        let storeCodesFile : string = "StoreCodes.csv"

        printfn("Enter the path of the store file");
        let path = Console.ReadLine();

        use stream = new StreamReader(@"" + path + storeCodesFile)


        let folderPath: string = "StoreData"
        let storeCodesFile: string = "StoreCodes.csv"
        let mutable valid = true


        let sw = new Stopwatch()
        sw.Start()

        while (valid) do
            let line = stream.ReadLine()
            if (line = null) then
                valid <- false
            else
                let lineSplit = line.Split ','
                let store: Store = new Store(lineSplit.[0], lineSplit.[1])
                stores.Add(lineSplit.[0], store)


        let fileNames: string[] = Directory.GetFiles(@"" + path + folderPath)

        let doParallelForEach =
            Parallel.ForEach(fileNames, (fun file ->
                let fileName: string = Path.GetFileNameWithoutExtension(file)
                let fileNameSplit: string[] = fileName.Split '_'

                let store: Store = stores.[fileNameSplit.[0]]
                let date: Date = new Date(Convert.ToInt32(fileNameSplit.[1]), Convert.ToInt32(fileNameSplit.[2]))
                dates.Enqueue(date)

                let data: string[] = File.ReadAllLines(file)
                for s in data do
                    let fileData: string[] = s.Split ','

                    let order: Order = new Order(store, date, fileData.[0], fileData.[1], Convert.ToDouble(fileData.[2]))
                    orders.Enqueue(order)
        ))

        sw.Stop();

        printfn "Time: %A" sw.Elapsed
        printfn "Order: %d" orders.Count
        printfn "dates: %d" dates.Count
        printfn "stores: %d" stores.Keys.Count


[<EntryPoint>]
let main argv = 
    let data = new Data()
    data.ReadAllData()


    MainLoop()
    printfn "Goodbye"
    Console.ReadKey()
    0 // return an integer exit code