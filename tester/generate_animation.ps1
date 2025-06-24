#
# Script to generate a high-quality animated sequence showing arrows flowing in your PlantUML diagram

# Path to your PlantUML file
$pumlFile = "Steps.puml"

# Create a temporary directory for individual frames
$tempDir = "temp_frames"
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null

# Read the original PUML file
$originalPuml = Get-Content $pumlFile -Raw

# Create more frames for smoother animation
$frameCount = 5

# Generate frames with different arrow styles
for ($i = 1; $i -le $frameCount; $i++) {
    Write-Host "Generating frame $i of $frameCount..."
    
    # Create a modified version of the PUML with different arrow styles
    $tempPuml = $originalPuml
    
    # Remove autonumber directive to avoid numbering in animation
    $tempPuml = $tempPuml -replace "autonumber.*", ""
    
    # Add high-resolution settings
    $tempPuml = $tempPuml -replace "@startuml DataMigrationProcess", "@startuml frame_$i`r`n!pragma resolution 300"
    
    # For each frame, use a different arrow style to simulate movement
    switch ($i) {
        1 { 
            # Frame 1: Dots at the beginning (sparse)
            $tempPuml = $tempPuml -replace "-\[#([a-zA-Z0-9]+),bold\]->", "-[dotted,#`$1,bold,thickness=1]->"
        }
        2 { 
            # Frame 2: Dots closer together
            $tempPuml = $tempPuml -replace "-\[#([a-zA-Z0-9]+),bold\]->", "-[dotted,#`$1,bold,thickness=2]->"
        }
        3 { 
            # Frame 3: Dashed (beginning of connection)
            $tempPuml = $tempPuml -replace "-\[#([a-zA-Z0-9]+),bold\]->", "-[dashed,#`$1,bold,thickness=2]->"
        }
        4 { 
            # Frame 4: Dashed (more pronounced)
            $tempPuml = $tempPuml -replace "-\[#([a-zA-Z0-9]+),bold\]->", "-[dashed,#`$1,bold,thickness=3]->"
        }
        5 { 
            # Frame 5: Solid (completion)
            $tempPuml = $tempPuml -replace "-\[#([a-zA-Z0-9]+),bold\]->", "-[#`$1,bold,thickness=3]->"
        }
    }
    
    # Save the modified PUML
    $tempPumlPath = Join-Path $tempDir "frame_$i.puml"
    Set-Content -Path $tempPumlPath -Value $tempPuml
    
    # Search for PlantUML jar in common locations or use current directory
    $plantUmlPaths = @(
        "C:\ProgramData\chocolatey\lib\plantuml\tools\plantuml.jar",
        ".\plantuml.jar",
        "C:\tools\plantuml\plantuml.jar",
        "${env:USERPROFILE}\.vscode\extensions\jebbs.plantuml-*\plantuml.jar"
    )
    
    $plantUmlJar = $plantUmlPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
    
    if (-not $plantUmlJar) {
        Write-Host "PlantUML.jar not found. Please download it from https://plantuml.com/download and place in the current directory."
        exit 1
    }
    
    # Generate the PNG with error handling - use higher memory settings for better rendering
    $pumlOutput = java -Xmx1024m -jar $plantUmlJar -tpng $tempPumlPath 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error generating frame $i. Examine the PUML file for syntax errors: $tempPumlPath" -ForegroundColor Red
        Write-Host "PlantUML output: $pumlOutput" -ForegroundColor Red
        continue
    }
    
    # Find the generated PNG
    $filename = "frame_$i"
    $generatedPngPath = Join-Path $tempDir "$filename.png"
    
    if (-not (Test-Path $generatedPngPath)) {
        # Try to find the file with a different name
        $possiblePngFiles = Get-ChildItem -Path $tempDir -Filter "*.png" | 
            Where-Object { $_.LastWriteTime -gt (Get-Date).AddSeconds(-5) }
        
        if ($possiblePngFiles.Count -gt 0) {
            $latestPng = $possiblePngFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
            Move-Item -Path $latestPng.FullName -Destination $generatedPngPath -Force
            Write-Host "  Renamed $($latestPng.Name) to $filename.png"
        } else {
            Write-Host "  WARNING: Could not find generated PNG file for frame $i" -ForegroundColor Yellow
        }
    }
}

# Use FFmpeg to create an animated GIF with improved quality settings
Write-Host "Creating high-quality animated GIF..."

# Check if FFmpeg is installed
if (Get-Command "ffmpeg" -ErrorAction SilentlyContinue) {
    # Create an animated GIF with FFmpeg - with high quality settings
    # Use palette generation for better quality
    ffmpeg -f image2 -framerate 3 -i "$tempDir\frame_%d.png" -vf "fps=12,scale=trunc(iw/2)*2:trunc(ih/2)*2,split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse" -loop 0 "DataMigrationProcess_animated.gif"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "High-quality animation created successfully: DataMigrationProcess_animated.gif"
    } else {
        Write-Host "Error creating animation" -ForegroundColor Red
    }
} else {
    Write-Host "FFmpeg not found. Please install FFmpeg to create the animated GIF." -ForegroundColor Yellow
    Write-Host "Individual frames are available in the $tempDir directory."
}