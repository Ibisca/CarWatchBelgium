import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/LoginView.vue'),
    meta: { layout: 'AuthLayout', requiresAuth: false }
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { layout: 'AuthLayout', requiresAuth: false }
  },
  {
    path: '/',
    name: 'Dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
  {
    path: '/dashboard',
    redirect: '/'
  },
  {
    path: '/saved-searches',
    name: 'SavedSearches',
    component: () => import('@/views/SavedSearchesView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
  {
    path: '/saved-searches/:id',
    name: 'SavedSearchDetails',
    component: () => import('@/views/SavedSearchDetailsView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
  {
    path: '/listings/:id',
    name: 'ListingDetails',
    component: () => import('@/views/ListingDetailsView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
  {
    path: '/favorites',
    name: 'Favorites',
    component: () => import('@/views/FavoritesView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
  {
    path: '/unseen',
    name: 'Unseen',
    component: () => import('@/views/UnseenListingsView.vue'),
    meta: { layout: 'AppLayout', requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

// Auth guard
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if ((to.path === '/login' || to.path === '/register') && isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
