import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'
import fs from 'fs'
import path from 'path'

export default defineConfig({
  plugins: [
    vue(),
    {
      name: 'copy-manifest',
      writeBundle() {
        const manifestSrc = resolve(__dirname, 'manifest.json')
        const manifestDest = resolve(__dirname, 'dist/manifest.json')
        const iconSrc = resolve(__dirname, 'public')

        // Copy manifest.json
        if (fs.existsSync(manifestSrc)) {
          fs.copyFileSync(manifestSrc, manifestDest)
          console.log('✓ Copied manifest.json to dist/')
        }

        // Create placeholder icons if they don't exist
        if (fs.existsSync(iconSrc)) {
          const files = fs.readdirSync(iconSrc)
          for (const file of files) {
            const src = path.join(iconSrc, file)
            const dest = path.join(resolve(__dirname, 'dist'), file)
            fs.copyFileSync(src, dest)
          }
        }
      },
    },
  ],
  build: {
    target: 'ES2020',
    minify: true,
    rollupOptions: {
      input: {
        popup: resolve(__dirname, 'src/popup/index.html'),
        background: resolve(__dirname, 'src/background/serviceWorker.ts'),
        content: resolve(__dirname, 'src/content/contentScript.ts'),
      },
      output: {
        entryFileNames: '[name].js',
        chunkFileNames: '[name].js',
        assetFileNames: '[name].[ext]',
      },
    },
    outDir: 'dist',
    emptyOutDir: true,
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
    },
  },
})
