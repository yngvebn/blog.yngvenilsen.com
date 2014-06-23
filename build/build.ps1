$thisDir = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

$startDir = Join-Path -path $thisDir -childPath '..\'

$files =  Get-ChildItem -r *.js -exclude *.min.*

foreach($file in $files){
    Write-Host Uglifying $file.Name
    $path = Join-Path -path $file.Directory -childpath $file.BaseName
    $output = $path + '.min.js'

    ng-annotate $file -ar --single_quotes -o $output
    uglifyjs $output -m -c  --screw-ie8 -o $output 
}

Write-Host Done.