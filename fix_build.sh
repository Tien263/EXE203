#!/bin/bash
echo "🧹 Cleaning up junk files..."

# Fix the specific build error
rm -f src/Program.csecProgram.cs
rm -f Program.csecProgram.cs

# Find and delete any other files with this pattern
find . -name "*csec*" -type f -delete

echo "✅ Cleanup complete. You can now run docker-compose up --build"
