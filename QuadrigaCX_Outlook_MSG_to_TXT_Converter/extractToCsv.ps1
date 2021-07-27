
class TradeRecord {
    [string]$pair1
    [string]$pair2
    [string]$sellOrder
    [string]$buyAmount
    [string]$buyCurrency
    [string]$sellAmmount
    [string]$sellCurrency
    [string]$dateUtc
    [string]$comments
    [string]$fee
    [string]$feeCurrency
}

class DepositWithdrawalCombinedRecord {

    [string]$Type
    [string]$ammount
    [string]$currency
    [string]$dateUtc
    [string]$comments
    [string]$fee
    [string]$feeCurrency
}
$ltc = 'ŁTC'
$eth = 'ΞTH'
$btc = 'XɃT'

$trades = New-Object 'System.Collections.Generic.List[TradeRecord]'
$deposits = New-Object 'System.Collections.Generic.List[DepositWithdrawalCombinedRecord]'
$withdrawals = New-Object 'System.Collections.Generic.List[DepositWithdrawalCombinedRecord]'
$partials = New-Object 'System.Collections.Generic.List[TradeRecord]'
$depositsWithdrawalsCombined = New-Object 'System.Collections.Generic.List[DepositWithdrawalCombinedRecord]'

$fullAndPartialsCombined = New-Object 'System.Collections.Generic.List[TradeRecord]'

$filledOrders = Get-ChildItem '.\Data\FilledOrders\*.txt'
$partialOrders = Get-ChildItem '.\Data\PartialOrders\*.txt'
$fundsAdded = Get-ChildItem '.\Data\FundsAdded\*.txt'
$withdrawRequests = Get-ChildItem '.\Data\Withdrawal\*.txt'

function PopulateTrades {
    param (
        $paths
    )   

    foreach ($f in $paths) {

        $content = Get-Content $f
        $pairs = [Regex]::Match($content[5].Replace($ltc, 'ltc').Replace($eth, 'eth').Replace($btc, 'btc'), `
                '^(.*?)\/(.*?) Sell Order ID# ([\d]*).* for ([\$\.\d].*?)([A-Za-z].*)$')

        "1 $($pairs.Groups[1].Value)"
        "2 $($pairs.Groups[2].Value)"
        "3 $($pairs.Groups[3].Value)"
        "4 $($pairs.Groups[4].Value)"
        "5 $($pairs.Groups[5].Value)"

        $p1 = $pairs.Groups[1].Value
        $p2 = $pairs.Groups[2].Value
        $orderNumber = $pairs.Groups[3].Value
        $ammount = [RegEx]::Replace($pairs.Groups[4].Value, '^[^\d]{1}', [string]::Empty)
        $currency = $pairs.Groups[5].Value

        $trade = [TradeRecord]::new()
        $trade.dateUtc = $f.CreationTimeUtc
        $trade.pair1 = $p1
        $trade.pair2 = $p2
        $trade.sellOrder = $orderNumber
        $trade.buyAmount = $ammount
        $trade.buyCurrency = $currency
        
        if ($p1 -eq $currency){
            $trade.sellCurrency = $p2
        }
        else {
            $trade.sellCurrency = $p1
        }
        $trade.comments = $content[5]

        $trades.Add($trade)    
    }
}

function PopulatePartialTrades {
    param (
        $paths
    )   

    foreach ($f in $paths) {

        $content = Get-Content $f
        $pairs = [Regex]::Match($content[5].Replace($ltc, 'ltc').Replace($eth, 'eth').Replace($btc, 'btc'), `
                '^(.*?)\/(.*?) Sell Order ID# ([\d]*).* for ([\$\.\d].*?)([A-Za-z].*)$')

        "1 $($pairs.Groups[1].Value)"
        "2 $($pairs.Groups[2].Value)"
        "3 $($pairs.Groups[3].Value)"
        "4 $($pairs.Groups[4].Value)"
        "5 $($pairs.Groups[5].Value)"

        $p1 = $pairs.Groups[1].Value
        $p2 = $pairs.Groups[2].Value
        $orderNumber = $pairs.Groups[3].Value
        $ammount = [RegEx]::Replace($pairs.Groups[4].Value, '^[^\d]{1}', [string]::Empty)
        $currency = $pairs.Groups[5].Value

        $partial = [TradeRecord]::new()
        $partial.dateUtc = $f.CreationTimeUtc
        $partial.pair1 = $p1
        $partial.pair2 = $p2
        $partial.sellOrder = $orderNumber
        $partial.buyAmount = $ammount
        $partial.buyCurrency = $currency
        
        if ($p1 -eq $currency){
            $partial.sellCurrency = $p2
        }
        else {
            $partial.sellCurrency = $p1
        }

        $partial.comments = $content[5]

        $partials.Add($partial)    
    }
}
function PopulateFundsAdded {
    param (
        $paths
    )

    foreach ($f in $paths) {
        $content = Get-Content $f

        $data = [Regex]::Match($content[4].Replace($ltc, 'ltc').Replace($eth, 'eth').Replace($btc, 'btc'), `
                '^([\d].*?)([A-Za-z].*?) for')

                $data.Groups
        "1 $($data.Groups[1].Value)"

#        $ammount = $data.Groups[1].Value
        $ammount = [RegEx]::Replace($data.Groups[1].Value, '^[^\d]{1}', [string]::Empty)
        $currency = $data.Groups[2].Value
        $comment = $content[4]

        $deposit = [DepositWithdrawalCombinedRecord]::new()
        $deposit.dateUtc = $f.CreationTimeUtc
        $deposit.ammount = $ammount
        $deposit.currency = $currency
        $deposit.comments = $comment
        
        $deposits.Add($deposit)    
    }
}

function PopulateWithdrawals {
    param (
        $paths
    )

    foreach ($f in $paths) {
        $content = Get-Content $f
        $data = [Regex]::Match($content[4].Replace($ltc, 'ltc').Replace($eth, 'eth').Replace($btc, 'btc'), `
                '^Your request to withdraw ([\$\d\.].*?)([A-Za-z].*?) using')

                $data.Groups
        "1 $($data.Groups[1].Value)"

        $ammount = $data.Groups[1].Value
        $ammount = [RegEx]::Replace($ammount, '^[^\d]{1}', [string]::Empty)
        $currency = $data.Groups[2].Value
        $comment = $content[4]

        $withdrawal = [DepositWithdrawalCombinedRecord]::new()
        $withdrawal.dateUtc = $f.CreationTimeUtc
        $withdrawal.ammount = $ammount
        $withdrawal.currency = $currency
        $withdrawal.comments = $comment
        
        $withdrawals.Add($withdrawal)    
    }
}

PopulateTrades $filledOrders
PopulateFundsAdded $fundsAdded
PopulateWithdrawals $withdrawRequests
PopulatePartialTrades $partialOrders


$fullAndPartialsCombined.AddRange($trades)
$fullAndPartialsCombined.AddRange($partials)

foreach ($w in $withdrawals){
     $w.Type = 'Withdrawal'

    $depositsWithdrawalsCombined.Add($w)
}

foreach ($d in $deposits){
    $d.Type = 'Deposit'
    $depositsWithdrawalsCombined.Add($d)
}

$fullAndPartialsCombined | Export-Csv .\Data\FinalReports\finished_and_partial_trades_combined.csv -Encoding utf8BOM
$trades | Sort-Object -Property dateUtc | Export-Csv .\Data\FinalReports\trades.csv -Encoding utf8BOM
$deposits | Sort-Object -Property dateUtc | Export-Csv .\Data\FinalReports\deposits.csv -Encoding utf8BOM
$withdrawals | Sort-Object -Property dateUtc | Export-Csv .\Data\FinalReports\withdrawals.csv -Encoding utf8BOM
$partials | Sort-Object -Property dateUtc | Export-Csv .\Data\FinalReports\partial_trades.csv -Encoding utf8BOM
$depositsWithdrawalsCombined | Sort-Object -Property dateUtc | Export-Csv .\Data\FinalReports\deposits_withdrawals_combined.csv -Encoding utf8BOM