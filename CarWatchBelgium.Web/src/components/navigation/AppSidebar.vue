<template>
  <aside class="sidebar">
    <nav class="nav-menu">
      <router-link to="/" class="nav-item" :class="{ active: route.name === 'Dashboard' }">
        <i class="pi pi-home"></i>
        <span>Dashboard</span>
      </router-link>
      <router-link to="/saved-searches" class="nav-item" :class="{ active: route.name === 'SavedSearches' }">
        <i class="pi pi-search"></i>
        <span>Saved Searches</span>
      </router-link>
      <router-link to="/unseen" class="nav-item" :class="{ active: route.name === 'Unseen' }">
        <i class="pi pi-eye"></i>
        <span>Unseen</span>
      </router-link>
      <router-link to="/favorites" class="nav-item" :class="{ active: route.name === 'Favorites' }">
        <i class="pi pi-heart"></i>
        <span>Favorites</span>
      </router-link>
    </nav>
  </aside>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<style scoped>
.sidebar {
  width: 250px;
  background: white;
  border-right: 1px solid #e0e0e0;
  overflow-y: auto;
  padding: 1rem 0;
}

.nav-menu {
  display: flex;
  flex-direction: column;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1.5rem;
  color: #666;
  text-decoration: none;
  transition: all 0.3s ease;
  border-left: 3px solid transparent;
}

.nav-item:hover {
  background-color: #f5f5f5;
  color: #667eea;
}

.nav-item.active {
  background-color: #f0f4ff;
  color: #667eea;
  border-left-color: #667eea;
  font-weight: 600;
}

.nav-item i {
  font-size: 1.1rem;
}

@media (max-width: 768px) {
  .sidebar {
    width: 60px;
  }

  .nav-item span {
    display: none;
  }

  .nav-item {
    justify-content: center;
    padding: 0.75rem;
  }
}
</style>
