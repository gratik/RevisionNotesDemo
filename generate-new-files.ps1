# File Generator for Revision Notes Project
# Creates all new high and medium priority topic files

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm"

function Create-FileWithContent {
    param(
        [string]$Path,
        [string]$Content
    )
    
    $Content | Set-Content -Path $Path -Encoding UTF8
    Write-Host "  ✓ Created: $Path" -ForegroundColor Green
}

Write-Host "`n=== Generating ALL New Topic Files ===" -ForegroundColor Cyan
Write-Host "Generated: $timestamp`n" -ForegroundColor Yellow

# Track progress
$fileCount = 0

