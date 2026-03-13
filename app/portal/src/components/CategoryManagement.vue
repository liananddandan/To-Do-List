<template>
  <div class="page">
    <div class="shell">
      <section class="header-card">
        <div>
          <p class="eyebrow">Categories</p>
          <h1>Keep the structure clean.</h1>
          <p>Rename, create, or remove categories. Tasks from deleted categories are preserved by moving to the default category.</p>
        </div>
        <button class="primary-btn" @click="openCreateDialog">Create category</button>
      </section>

      <div v-if="isLoading" class="state-card">Loading categories...</div>
      <div v-else-if="errorMessage" class="state-card error-state">{{ errorMessage }}</div>
      <div v-else class="category-grid">
        <article v-for="category in categoriesWithCounts" :key="category.id" class="category-card">
          <div class="category-top">
            <div>
              <h2>
                {{ category.name }}
                <span v-if="category.isDefault" class="default-badge">Default</span>
              </h2>
              <p v-if="category.description">{{ category.description }}</p>
            </div>
            <span class="count-badge">{{ category.taskCount }} task<span v-if="category.taskCount !== 1">s</span></span>
          </div>

          <div class="meta-row">
            <span>Created {{ formatDate(category.createdAt) }}</span>
          </div>

          <div class="card-actions">
            <button class="ghost-btn" @click="openEditDialog(category)">Edit</button>
            <button class="danger-btn" @click="deleteCategory(category)" :disabled="category.isDefault">
              Delete
            </button>
          </div>
        </article>
      </div>
    </div>

    <div v-if="isDialogOpen" class="overlay" @click="closeDialog"></div>

    <div v-if="isDialogOpen" class="modal-shell">
      <AddCategoryDialog
        :mode="dialogMode"
        :category="selectedCategory"
        @close="closeDialog"
        @saved="handleSaved"
      />
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import api from '@/tools/api-request.js'
import AddCategoryDialog from './AddCategoryDialog.vue'

const categories = ref([])
const taskGroups = ref([])
const isLoading = ref(false)
const errorMessage = ref('')
const isDialogOpen = ref(false)
const dialogMode = ref('create')
const selectedCategory = ref(null)

const categoriesWithCounts = computed(() => {
  const counts = new Map(
    taskGroups.value.map(category => [category.id, category.tasks?.length || 0])
  )

  return categories.value.map(category => ({
    ...category,
    taskCount: counts.get(category.id) || 0
  }))
})

const fetchData = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const [categoryResponse, taskResponse] = await Promise.all([
      api.get('/api/categories'),
      api.get('/api/tasks')
    ])

    categories.value = categoryResponse.data?.data?.info ?? []
    taskGroups.value = taskResponse.data?.data?.info ?? []
  } catch (err) {
    errorMessage.value = 'Failed to load categories.'
  } finally {
    isLoading.value = false
  }
}

const openCreateDialog = () => {
  dialogMode.value = 'create'
  selectedCategory.value = null
  isDialogOpen.value = true
}

const openEditDialog = (category) => {
  dialogMode.value = 'edit'
  selectedCategory.value = { ...category }
  isDialogOpen.value = true
}

const closeDialog = () => {
  isDialogOpen.value = false
  selectedCategory.value = null
}

const handleSaved = async () => {
  closeDialog()
  await fetchData()
}

const deleteCategory = async (category) => {
  if (category.isDefault) return

  const confirmed = window.confirm(`Delete "${category.name}"? Tasks will be moved to the default category.`)
  if (!confirmed) return

  try {
    await api.delete(`/api/categories/${category.id}`)
    await fetchData()
  } catch (err) {
    errorMessage.value = err.response?.data?.data?.code === 300114
      ? 'The default category cannot be deleted.'
      : 'Failed to delete category.'
  }
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  if (Number.isNaN(date.getTime())) return ''

  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

onMounted(fetchData)
</script>

<style scoped>
.page {
  min-height: calc(100vh - 70px);
  padding: 32px 20px 48px;
  background: var(--page-background);
}

.shell {
  max-width: 1120px;
  margin: 0 auto;
}

.header-card {
  display: flex;
  justify-content: space-between;
  gap: 24px;
  padding: 28px;
  border-radius: 28px;
  background: var(--surface);
  border: 1px solid var(--border);
  box-shadow: var(--shadow-soft);
  margin-bottom: 24px;
}

.eyebrow {
  margin-bottom: 10px;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--accent);
  font-weight: 700;
}

.header-card h1 {
  font-size: 34px;
  font-weight: 800;
}

.header-card p {
  margin-top: 10px;
  max-width: 700px;
  color: var(--text-secondary);
}

.primary-btn {
  align-self: flex-start;
  border: none;
  border-radius: 14px;
  padding: 12px 18px;
  background: linear-gradient(135deg, #48bb78 0%, #2f855a 100%);
  color: white;
  font-weight: 700;
}

.state-card {
  padding: 24px;
  border-radius: 22px;
  background: var(--surface);
  border: 1px solid var(--border);
  box-shadow: var(--shadow-soft);
}

.error-state {
  color: var(--danger);
  background: #fff7ed;
  border-color: #fed7aa;
}

.category-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 18px;
}

.category-card {
  padding: 22px;
  border-radius: 24px;
  background: var(--surface);
  border: 1px solid var(--border);
  box-shadow: var(--shadow-soft);
}

.category-top {
  display: flex;
  justify-content: space-between;
  gap: 12px;
}

.category-top h2 {
  font-size: 22px;
  font-weight: 800;
}

.category-top p {
  margin-top: 8px;
  color: var(--text-secondary);
}

.default-badge,
.count-badge {
  display: inline-flex;
  align-items: center;
  border-radius: 999px;
  padding: 6px 10px;
  font-size: 12px;
  font-weight: 700;
}

.default-badge {
  margin-left: 10px;
  background: var(--brand-soft);
  color: var(--brand-strong);
}

.count-badge {
  background: #edf7f5;
  color: var(--accent);
  white-space: nowrap;
}

.meta-row {
  margin-top: 16px;
  color: var(--text-tertiary);
}

.card-actions {
  margin-top: 18px;
  display: flex;
  gap: 12px;
}

.ghost-btn,
.danger-btn {
  border: none;
  border-radius: 12px;
  padding: 10px 12px;
  font-weight: 700;
}

.ghost-btn {
  background: var(--surface-muted);
  color: var(--text-primary);
}

.danger-btn {
  background: var(--danger-soft);
  color: var(--danger);
}

.danger-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  backdrop-filter: blur(4px);
  z-index: 80;
}

.modal-shell {
  position: fixed;
  inset: 0;
  display: grid;
  place-items: center;
  padding: 20px;
  z-index: 81;
}

.modal-shell :deep(.dialog-card) {
  width: min(640px, 100%);
}

@media (max-width: 820px) {
  .header-card,
  .category-grid {
    grid-template-columns: 1fr;
  }

  .header-card {
    flex-direction: column;
  }
}

@media (max-width: 640px) {
  .page {
    padding: 20px 14px 32px;
  }

  .header-card h1 {
    font-size: 28px;
  }

  .category-top {
    flex-direction: column;
  }
}
</style>
