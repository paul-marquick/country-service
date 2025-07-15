
$body = @{
    id = "3b2d21c5-38ac-4151-803e-df872ae53d5d"
    name = "Sport"
}

$parameters = @{
    Method = 'POST'
    Uri = 'http://localhost:5581/category'
    Body =  ConvertTo-JSON($body)
    ContentType = 'application/json'
}

Invoke-WebRequest @parameters
