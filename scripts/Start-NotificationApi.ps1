$ErrorActionPreference = "Stop"

$thuMucGoc = Split-Path -Parent $PSScriptRoot
$tepMoiTruong = Join-Path $thuMucGoc ".env"

if (-not (Test-Path $tepMoiTruong)) {
    throw "Khong tim thay file .env. Hay tao file nay tu .env.example va dat mat khau RabbitMQ."
}

$giaTriMoiTruong = @{}
foreach ($dong in Get-Content $tepMoiTruong) {
    if ($dong -match '^\s*([^#=]+?)\s*=\s*(.*?)\s*$') {
        $giaTriMoiTruong[$Matches[1]] = $Matches[2]
    }
}

if (-not $giaTriMoiTruong.RABBITMQ_DEFAULT_USER -or -not $giaTriMoiTruong.RABBITMQ_DEFAULT_PASS) {
    throw "File .env can co RABBITMQ_DEFAULT_USER va RABBITMQ_DEFAULT_PASS."
}

$env:RabbitMq__UserName = $giaTriMoiTruong.RABBITMQ_DEFAULT_USER
$env:RabbitMq__Password = $giaTriMoiTruong.RABBITMQ_DEFAULT_PASS

Push-Location $thuMucGoc
try {
    dotnet run --project "src\Notification.Api\Notification.Api.csproj" --launch-profile http
    if ($LASTEXITCODE -ne 0) {
        throw "Notification.Api dung voi ma loi $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}
