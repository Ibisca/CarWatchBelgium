#!/bin/bash
# Quick setup script for CarWatch Belgium Extension

echo "🚀 CarWatch Belgium Extension Setup"
echo "===================================="
echo ""

# Check Node.js
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18+"
    exit 1
fi

echo "✓ Node.js $(node --version) found"
echo ""

# Install dependencies
echo "📦 Installing dependencies..."
npm install
echo ""

# Build extension
echo "🔨 Building extension..."
npm run build
echo ""

# Check if build succeeded
if [ -d "dist" ] && [ -f "dist/manifest.json" ]; then
    echo "✅ Build successful!"
    echo ""
    echo "📂 Extension is ready in 'dist/' folder"
    echo ""
    echo "Next steps:"
    echo "1. Open Chrome: chrome://extensions/"
    echo "2. Enable 'Developer mode'"
    echo "3. Click 'Load unpacked'"
    echo "4. Select the 'dist/' folder"
    echo ""
    echo "For Edge:"
    echo "1. Open Edge: edge://extensions/"
    echo "2. Enable 'Developer mode'"
    echo "3. Click 'Load unpacked'"
    echo "4. Select the 'dist/' folder"
else
    echo "❌ Build failed! Check errors above."
    exit 1
fi
