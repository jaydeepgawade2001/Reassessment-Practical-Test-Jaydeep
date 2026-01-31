# Base URL
$baseUrl = "http://localhost:5200/api"

# 1. Create 5 Rooms
Write-Host "Creating 5 Rooms..." -ForegroundColor Cyan
for ($i = 1; $i -le 5; $i++) {
    $body = @{
        name = "Stress Test Room $i"
        capacity = 10 + $i
        location = "Floor $i"
    } | ConvertTo-Json

    try {
        Invoke-RestMethod -Method Post -Uri "$baseUrl/Rooms" -ContentType "application/json" -Body $body
        Write-Host "Created Room $i" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to create Room $i" -ForegroundColor Red
        Write-Host $_
    }
}

# 2. Create 50 Bookings (10 per room)
Write-Host "`nCreating 50 Bookings..." -ForegroundColor Cyan
$startDate = (Get-Date).AddDays(1).Date # Start tomorrow at 00:00

for ($r = 1; $r -le 5; $r++) { # For each of the 5 rooms (assuming IDs 2-6, since 1 exists)
    # Note: Room IDs might be 2,3,4,5,6 if 1 already exists. Let's assume sequential IDs based on creation order.
    # To be safe, we'll target the IDs we just likely created. We know 1 exists. We created 5 more.
    # IDs should be roughly 1, 2, 3, 4, 5, 6.
    
    $targetRoomId = $r + 1 # Targeting new rooms mainly

    for ($b = 0; $b -lt 10; $b++) {
        $start = $startDate.AddHours($b + 9) # 9 AM, 10 AM, etc.
        $end = $start.AddHours(1)
        
        $body = @{
            roomId = $targetRoomId
            title = "Stress Meeting $b"
            startTime = $start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            endTime = $end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            createdBy = "LoadTester"
        } | ConvertTo-Json

        try {
            Invoke-RestMethod -Method Post -Uri "$baseUrl/Bookings" -ContentType "application/json" -Body $body
            Write-Host "Created Booking $b for Room $targetRoomId" -ForegroundColor Gray
        }
        catch {
             Write-Host "Failed Booking $b for Room $targetRoomId" -ForegroundColor Red
             # Write-Host $_.Exception.Response.StatusCode
        }
    }
}
Write-Host "`nDone seeding data!" -ForegroundColor Green
