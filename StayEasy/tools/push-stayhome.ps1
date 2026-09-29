# ============================================================================
# StayEasy - Sync code len repo stayhome (https://github.com/Duongcute2604/stayhome)
# ============================================================================
# Cach dung:
#   powershell -ExecutionPolicy Bypass -File tools/push-stayhome.ps1 -Message "feat: mo ta ngan gon"
# Lam gi:
#   1. Copy toan bo StayEasy/ (tru .git, node_modules, bin/obj, dist, .env)
#      vao ban clone tam o $env:TEMP\opencode\stayhome-check
#   2. Commit + push len nhanh main cua repo stayhome
# Tai sao clone rieng: tranh day nham cac project khac trong monorepo bai-tap-lon
# ============================================================================
param(
  [string]$Message = "chore: dong bo StayEasy"
)

$ErrorActionPreference = "Stop"
$StayEasyDir = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$CloneDir = Join-Path ([System.IO.Path]::GetTempPath()) "opencode\stayhome-check"
$Remote = "https://github.com/Duongcute2604/stayhome.git"

if (-not (Test-Path (Join-Path $CloneDir ".git"))) {
  Write-Host "[sync] Clone moi stayhome..."
  New-Item -ItemType Directory -Force -Path (Split-Path $CloneDir -Parent) | Out-Null
  git clone $Remote $CloneDir
}

Write-Host "[sync] Copy file StayEasy -> clone tam..."
robocopy $StayEasyDir $CloneDir /MIR /XD .git node_modules bin obj dist .vs .idea /XF .env | Out-Null

Push-Location $CloneDir
try {
  git add -A
  $staged = git diff --cached --name-only
  if (-not $staged) {
    Write-Host "[sync] Khong co thay doi gi, bo qua push."
    return
  }
  git commit -m $Message | Out-Null
  git push origin main
  Write-Host "[sync] Push xong."
} finally {
  Pop-Location
}
