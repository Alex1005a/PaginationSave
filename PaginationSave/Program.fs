open System
open System.IO
open System.Text.Json
open HtmlAgilityPack

type SearchQuery =
    struct
        val teg: string
        val atr: string
        val atrValue: string
        new (t: string, a: string, av: string) = { teg = t; atr = a; atrValue = av}
    end

let loadHtml (pageUrl:string) = 
  let web = HtmlWeb()
  let htmldoc = web.Load(pageUrl)
  htmldoc
 
let Save (page:int, list:List<string>) =
    let json = JsonSerializer.Serialize(list)
    File.WriteAllText(String.Format("test{0}.json", page), json);       
    
let processPage (url:string, searchQuery:SearchQuery)  = 
  try
    let htmldoc = loadHtml url
    let query = String.Format(@"//{0}[@{1}='{2}']", searchQuery.teg, searchQuery.atr, searchQuery.atrValue)
    let nodes = htmldoc.DocumentNode.SelectNodes(query)
    let list = nodes |> Seq.toList |> List.map(fun x -> x.InnerText)
    printf "%A" list
    list
  with
    | ex -> 
      Console.WriteLine("Exception: " + ex.Message)
      []
      
      
let processSeveralPages(baseUrl:string, start:int, stop:int, searchQuery:SearchQuery) =
    Async.Parallel [
    for i in start .. stop do
        async {
            let page = processPage(String.Format(baseUrl, i), searchQuery)
            Save(i, page) |> ignore
        }
    ] |> Async.RunSynchronously
                   
[<EntryPoint>]
let main argv =
    let query = SearchQuery("a", "data-dy", "title")
    let url = "https://www.eldorado.ru/c/kompyutery/?page={0}"
    processSeveralPages(url, 1, 4, query) |> ignore
    0 
