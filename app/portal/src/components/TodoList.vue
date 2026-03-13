<template>
  <div class="page">
    <div class="todo-shell">
      <section class="hero-card">
        <div>
          <p class="eyebrow">Task workspace</p>
          <h1 class="page-title">Stay on top of every commitment.</h1>
          <p class="page-subtitle">
            Review tasks by category, update status quickly, and keep your list clean.
          </p>
        </div>

        <div class="hero-actions">
          <button class="primary-btn" @click="openCreateDialog">Create task</button>
          <router-link to="/categories" class="secondary-link">Manage categories</router-link>
        </div>
      </section>

      <section class="summary-grid">
        <article class="summary-card">
          <span class="summary-label">Total tasks</span>
          <strong>{{ taskSummary.total }}</strong>
        </article>
        <article class="summary-card">
          <span class="summary-label">Completed</span>
          <strong>{{ taskSummary.completed }}</strong>
        </article>
        <article class="summary-card">
          <span class="summary-label">Remaining</span>
          <strong>{{ taskSummary.pending }}</strong>
        </article>
      </section>

      <div v-if="isLoading" class="state-card">Loading tasks...</div>
      <div v-else-if="error" class="state-card error-state">{{ error }}</div>
      <div v-else-if="!taskGroups.length" class="state-card empty-state">
        <div class="empty-title">No tasks yet</div>
        <div class="empty-text">Create your first task to start building momentum.</div>
      </div>

      <div v-else class="category-grid">
        <section v-for="category in taskGroups" :key="category.id" class="category-card">
          <div class="category-header">
            <div>
              <h2 class="category-title">
                {{ category.name }}
                <span v-if="category.isDefault" class="default-badge">Default</span>
              </h2>
              <p v-if="category.description" class="category-description">{{ category.description }}</p>
            </div>

            <div class="task-count">{{ category.tasks.length }} item<span v-if="category.tasks.length !== 1">s</span></div>
          </div>

          <div v-if="category.tasks.length" class="task-list">
            <article v-for="task in category.tasks" :key="task.id" class="task-card" :class="{ done: task.isCompleted }">
              <button class="status-toggle" @click="toggleTaskCompletion(task)" :aria-label="task.isCompleted ? 'Mark incomplete' : 'Mark complete'">
                <span>{{ task.isCompleted ? '✓' : '' }}</span>
              </button>

              <div class="task-main">
                <div class="task-top">
                  <h3 class="task-title">{{ task.title }}</h3>
                  <span class="priority-badge" :class="getPriorityClass(task.priority)">
                    {{ getPriorityLabel(task.priority) }}
                  </span>
                </div>

                <p v-if="task.description" class="task-description">{{ task.description }}</p>

                <div class="task-meta">
                  <span v-if="task.dueDate" class="meta-chip">Due {{ formatDate(task.dueDate) }}</span>
                  <span class="meta-chip">{{ task.isCompleted ? 'Completed' : 'In progress' }}</span>
                  <span class="meta-chip">Created {{ formatDate(task.createdDate) }}</span>
                </div>
              </div>

              <div class="task-actions">
                <button class="ghost-btn" @click="openEditDialog(task)">Edit</button>
                <button class="danger-btn" @click="deleteTask(task)">Delete</button>
              </div>
            </article>
          </div>

          <div v-else class="empty-category">No tasks in this category yet.</div>
        </section>
      </div>
    </div>

    <div v-if="isDialogOpen" class="overlay" @click="closeDialog"></div>

    <div v-if="isDialogOpen" class="modal-shell">
      <AddTaskDialog
        :mode="dialogMode"
        :task="selectedTask"
        :categories="categories"
        @close="closeDialog"
        @saved="handleTaskSaved"
        @category-created="handleCategoryCreated"
      />
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import api from '@/tools/api-request.js'
import AddTaskDialog from './AddTaskDialog.vue'

const taskGroups = ref([])
const categories = ref([])
const isLoading = ref(false)
const error = ref('')
const isDialogOpen = ref(false)
const dialogMode = ref('create')
const selectedTask = ref(null)

const taskSummary = computed(() => {
  const tasks = taskGroups.value.flatMap(category => category.tasks || [])
  const completed = tasks.filter(task => task.isCompleted).length

  return {
    total: tasks.length,
    completed,
    pending: tasks.length - completed
  }
})

const fetchTasks = async () => {
  isLoading.value = true
  error.value = ''

  try {
    const response = await api.get('/api/tasks')
    taskGroups.value = response.data?.data?.info ?? []
  } catch (err) {
    taskGroups.value = []
    error.value = 'Failed to load tasks.'
  } finally {
    isLoading.value = false
  }
}

const fetchCategories = async () => {
  try {
    const response = await api.get('/api/categories')
    categories.value = response.data?.data?.info ?? []
  } catch (err) {
    categories.value = []
  }
}

const openCreateDialog = () => {
  dialogMode.value = 'create'
  selectedTask.value = null
  isDialogOpen.value = true
}

const openEditDialog = (task) => {
  dialogMode.value = 'edit'
  selectedTask.value = { ...task }
  isDialogOpen.value = true
}

const closeDialog = () => {
  isDialogOpen.value = false
  selectedTask.value = null
}

const handleTaskSaved = async () => {
  closeDialog()
  await Promise.all([fetchTasks(), fetchCategories()])
}

const handleCategoryCreated = async (createdCategory) => {
  if (createdCategory) {
    categories.value = [...categories.value, createdCategory]
  } else {
    await fetchCategories()
  }
}

const toggleTaskCompletion = async (task) => {
  try {
    await api.patch(`/api/tasks/${task.id}/completion`, {
      isCompleted: !task.isCompleted
    })
    await fetchTasks()
  } catch (err) {
    error.value = 'Failed to update task status.'
  }
}

const deleteTask = async (task) => {
  const confirmed = window.confirm(`Delete "${task.title}"?`)
  if (!confirmed) return

  try {
    await api.delete(`/api/tasks/${task.id}`)
    await fetchTasks()
  } catch (err) {
    error.value = 'Failed to delete task.'
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

const getPriorityLabel = (priority) => {
  if (priority >= 1000) return 'High'
  if (priority >= 500) return 'Medium'
  return 'Low'
}

const getPriorityClass = (priority) => {
  if (priority >= 1000) return 'high'
  if (priority >= 500) return 'medium'
  return 'low'
}

onMounted(async () => {
  await Promise.all([fetchTasks(), fetchCategories()])
})
</script>

<style scoped>
.page {
  min-height: calc(100vh - 70px);
  padding: 32px 20px 48px;
  background: var(--page-background);
}

.todo-shell {
  max-width: 1180px;
  margin: 0 auto;
}

.hero-card {
  display: flex;
  justify-content: space-between;
  gap: 24px;
  padding: 28px;
  border-radius: 28px;
  background: linear-gradient(135deg, rgba(20, 33, 61, 0.98) 0%, rgba(15, 118, 110, 0.92) 100%);
  color: white;
  box-shadow: var(--shadow-strong);
}

.eyebrow {
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: rgba(255, 255, 255, 0.72);
  margin-bottom: 10px;
}

.page-title {
  font-size: 36px;
  line-height: 1.1;
  font-weight: 800;
}

.page-subtitle {
  margin-top: 12px;
  max-width: 640px;
  color: rgba(255, 255, 255, 0.8);
}

.hero-actions {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.primary-btn,
.secondary-link {
  border: none;
  border-radius: 14px;
  padding: 12px 18px;
  font-weight: 700;
}

.primary-btn {
  background: #f8fafc;
  color: #14213d;
}

.secondary-link {
  background: rgba(255, 255, 255, 0.12);
  color: white;
}

.summary-grid {
  margin: 24px 0;
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
}

.summary-card,
.state-card,
.category-card {
  background: var(--surface);
  border: 1px solid var(--border);
  box-shadow: var(--shadow-soft);
}

.summary-card {
  padding: 20px 22px;
  border-radius: 22px;
}

.summary-label {
  display: block;
  color: var(--text-secondary);
  margin-bottom: 8px;
}

.summary-card strong {
  font-size: 30px;
  font-weight: 800;
  color: var(--text-primary);
}

.state-card {
  border-radius: 22px;
  padding: 28px;
}

.error-state {
  color: var(--danger);
  background: #fff7ed;
  border-color: #fed7aa;
}

.empty-state {
  text-align: center;
}

.empty-title {
  font-size: 22px;
  font-weight: 800;
}

.empty-text {
  margin-top: 8px;
  color: var(--text-secondary);
}

.category-grid {
  display: grid;
  gap: 20px;
}

.category-card {
  border-radius: 24px;
  padding: 24px;
}

.category-header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  align-items: flex-start;
  margin-bottom: 18px;
}

.category-title {
  font-size: 24px;
  font-weight: 800;
  color: var(--text-primary);
}

.default-badge,
.task-count {
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

.task-count {
  background: #edf7f5;
  color: var(--accent);
}

.category-description {
  margin-top: 8px;
  color: var(--text-secondary);
}

.task-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.task-card {
  display: grid;
  grid-template-columns: auto 1fr auto;
  gap: 16px;
  align-items: flex-start;
  padding: 18px;
  border-radius: 20px;
  background: var(--surface-muted);
  border: 1px solid #e5edf2;
}

.task-card.done {
  background: #f3faf8;
}

.status-toggle {
  width: 28px;
  height: 28px;
  border-radius: 999px;
  border: 2px solid #94a3b8;
  background: white;
  color: white;
  font-weight: 800;
}

.task-card.done .status-toggle {
  background: var(--brand);
  border-color: var(--brand);
}

.task-top {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: flex-start;
}

.task-title {
  font-size: 18px;
  font-weight: 800;
  color: var(--text-primary);
}

.task-card.done .task-title {
  text-decoration: line-through;
  color: var(--text-secondary);
}

.task-description {
  margin-top: 8px;
  color: var(--text-secondary);
}

.task-meta {
  margin-top: 12px;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.meta-chip,
.priority-badge {
  display: inline-flex;
  align-items: center;
  border-radius: 999px;
  padding: 6px 10px;
  font-size: 12px;
  font-weight: 700;
}

.meta-chip {
  background: white;
  color: var(--text-secondary);
  border: 1px solid var(--border);
}

.priority-badge.high {
  background: #fee2e2;
  color: #b91c1c;
}

.priority-badge.medium {
  background: #fef3c7;
  color: #b45309;
}

.priority-badge.low {
  background: #dcfce7;
  color: #166534;
}

.task-actions {
  display: flex;
  gap: 10px;
}

.ghost-btn,
.danger-btn {
  border: none;
  border-radius: 12px;
  padding: 10px 12px;
  font-weight: 700;
}

.ghost-btn {
  background: white;
  color: var(--text-primary);
  border: 1px solid var(--border);
}

.danger-btn {
  background: var(--danger-soft);
  color: var(--danger);
}

.empty-category {
  padding: 18px;
  border-radius: 18px;
  border: 1px dashed var(--border-strong);
  color: var(--text-secondary);
  background: rgba(255, 255, 255, 0.45);
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
  width: min(720px, 100%);
}

@media (max-width: 860px) {
  .hero-card,
  .task-card {
    grid-template-columns: 1fr;
  }

  .hero-card {
    padding: 24px;
  }

  .hero-actions,
  .task-actions {
    flex-wrap: wrap;
  }

  .summary-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .page {
    padding: 20px 14px 32px;
  }

  .page-title {
    font-size: 30px;
  }

  .category-header,
  .task-top {
    flex-direction: column;
  }
}
</style>
