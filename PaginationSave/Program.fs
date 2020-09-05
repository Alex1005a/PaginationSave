open System
open HtmlAgilityPack

type SearchQuery =
    struct
        val teg: string
        val atr: string
        val atrValue: string
        new (t: string, a: string, av: string) = { teg = t; atr = a; atrValue = av}
    end

let loadHtml (pageUrl:string) = 
  let web = HtmlWeb();
  let htmldoc = web.Load(pageUrl)
  htmldoc
  
let processHubPage (url:string, searchQuery:SearchQuery)  = 
  try
    let htmldoc = loadHtml url
    let query = String.Format(@"//{0}[@{1}='{2}']", searchQuery.teg, searchQuery.atr, searchQuery.atrValue)
    let nodes = htmldoc.DocumentNode.SelectNodes(query)    
    nodes |> Seq.toList|> List.map(fun x -> x.InnerText) 
  with
    | ex -> 
      Console.WriteLine("Exception: " + ex.Message) 
      []


[<EntryPoint>]
let main argv =
    let query = SearchQuery("a", "data-dy", "title")
    let url = ""
    printf "%A" (processHubPage(url, query))
    0 
