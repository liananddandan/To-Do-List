<template>
  <div class="dialog-card">
    <div class="dialog-header">
      <div>
        <h2>{{ isEditMode ? 'Edit task' : 'Create task' }}</h2>
        <p>{{ isEditMode ? 'Update the task details and status.' : 'Capture the next thing that needs attention.' }}</p>
      </div>
      <button class="icon-close" @click="emit('close')" type="button">×</button>
    </div>

    <form class="form" @submit.prevent="submitTask">
      <div class="form-group">
        <label for="task-title">Title <span class="required">*</span></label>
        <input id="task-title" v-model.trim="form.title" type="text" placeholder="Finish sprint planning notes" />
      </div>

      <div class="form-group">
        <label for="task-description">Description</label>
        <textarea
          id="task-description"
          v-model.trim="form.description"
          rows="4"
          placeholder="Add relevant details, links, or next steps."
        ></textarea>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="task-due-date">Due date</label>
          <input id="task-due-date" type="date" v-model="form.dueDate" />
        </div>

        <div class="form-group">
          <label for="task-priority">Priority</label>
          <select id="task-priority" v-model.number="form.priority">
            <option :value="1000">High</option>
            <option :value="500">Medium</option>
            <option :value="100">Low</option>
          </select>
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <div class="label-row">
            <label for="task-category">Category <span class="required">*</span></label>
            <button class="text-button" @click="toggleInlineCategory" type="button">
              {{ showInlineCategory ? 'Cancel' : '+ New category' }}
            </button>
          </div>

          <select id="task-category" v-model="form.categoryId">
            <option disabled value="">Select category</option>
            <option v-for="cat in categories" :key="cat.id" :value="String(cat.id)">
              {{ cat.name }}
            </option>
          </select>
        </div>

        <div v-if="isEditMode" class="form-group checkbox-group">
          <label class="checkbox-row">
            <input type="checkbox" v-model="form.isCompleted" />
            <span>Mark as completed</span>
          </label>
        </div>
      </div>

      <div v-if="showInlineCategory" class="inline-category-box">
        <div class="form-row">
          <div class="form-group">
            <label for="new-category-name">Category name</label>
            <input
              id="new-category-name"
              v-model.trim="newCategory.name"
              type="text"
              placeholder="Work, Home, Health"
            />
          </div>

          <div class="form-group">
            <label for="new-category-description">Description</label>
            <input
              id="new-category-description"
              v-model.trim="newCategory.description"
              type="text"
              placeholder="Optional short context"
            />
          </div>
        </div>

        <div class="inline-actions">
          <button class="secondary-btn" @click="showInlineCategory = false" type="button">Cancel</button>
          <button class="primary-btn" @click="createCategory" type="button" :disabled="isCreatingCategory">
            {{ isCreatingCategory ? 'Creating...' : 'Create category' }}
          </button>
        </div>

        <p v-if="categoryError" class="error-text">{{ categoryError }}</p>
      </div>

      <p v-if="taskError" class="error-text">{{ taskError }}</p>

      <div class="footer-actions">
        <button class="secondary-btn" @click="emit('close')" type="button">Cancel</button>
        <button class="primary-btn" type="submit" :disabled="isSubmitting">
          {{ isSubmitting ? 'Saving...' : isEditMode ? 'Save changes' : 'Create task' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { computed, reactive, ref, watch } from 'vue'
import api from '@/tools/api-request.js'

const props = defineProps({
  mode: {
    type: String,
    default: 'create'
  },
  task: {
    type: Object,
    default: null
  },
  categories: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['close', 'saved', 'category-created'])

const form = reactive({
  title: '',
  description: '',
  dueDate: '',
  priority: 1000,
  categoryId: '',
  isCompleted: false
})

const newCategory = reactive({
  name: '',
  description: ''
})

const showInlineCategory = ref(false)
const taskError = ref('')
const categoryError = ref('')
const isSubmitting = ref(false)
const isCreatingCategory = ref(false)

const isEditMode = computed(() => props.mode === 'edit')

const resetForm = () => {
  const fallbackCategory = props.categories.find(category => category.isDefault) ?? props.categories[0]

  form.title = props.task?.title || ''
  form.description = props.task?.description || ''
  form.dueDate = props.task?.dueDate ? String(props.task.dueDate).slice(0, 10) : ''
  form.priority = props.task?.priority ?? 1000
  form.categoryId = String(props.task?.categoryId ?? fallbackCategory?.id ?? '')
  form.isCompleted = props.task?.isCompleted ?? false
  taskError.value = ''
}

watch(
  () => [props.task, props.categories],
  () => resetForm(),
  { immediate: true, deep: true }
)

const toggleInlineCategory = () => {
  categoryError.value = ''
  showInlineCategory.value = !showInlineCategory.value
}

const createCategory = async () => {
  categoryError.value = ''

  if (!newCategory.name) {
    categoryError.value = 'Please enter a category name.'
    return
  }

  try {
    isCreatingCategory.value = true
    const response = await api.post('/api/categories', {
      name: newCategory.name,
      description: newCategory.description || null
    })

    const createdCategory = response.data?.data?.info
    if (!createdCategory) {
      categoryError.value = 'Failed to create category.'
      return
    }

    form.categoryId = String(createdCategory.id)
    newCategory.name = ''
    newCategory.description = ''
    showInlineCategory.value = false
    emit('category-created', createdCategory)
  } catch (err) {
    categoryError.value = err.response?.data?.data?.code === 300108
      ? 'Category name already exists.'
      : 'Failed to create category.'
  } finally {
    isCreatingCategory.value = false
  }
}

const submitTask = async () => {
  taskError.value = ''

  if (!form.title) {
    taskError.value = 'Task title is required.'
    return
  }

  if (!form.categoryId) {
    taskError.value = 'Please select a category.'
    return
  }

  const payload = {
    title: form.title,
    description: form.description || null,
    dueDate: form.dueDate || null,
    priority: form.priority,
    categoryId: form.categoryId,
    isCompleted: form.isCompleted
  }

  try {
    isSubmitting.value = true

    if (isEditMode.value && props.task?.id) {
      await api.put(`/api/tasks/${props.task.id}`, payload)
    } else {
      await api.post('/api/tasks', payload)
    }

    emit('saved')
  } catch (err) {
    taskError.value = err.response?.data?.data?.code === 300101
      ? 'Selected category no longer exists.'
      : 'Failed to save task.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.dialog-card {
  background: var(--surface-strong);
  border-radius: 24px;
  padding: 24px;
  box-shadow: var(--shadow-strong);
  border: 1px solid var(--border);
}

.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 20px;
}

.dialog-header h2 {
  font-size: 24px;
  font-weight: 800;
  color: var(--text-primary);
}

.dialog-header p {
  margin-top: 8px;
  color: var(--text-secondary);
  font-size: 14px;
}

.icon-close {
  border: none;
  background: var(--surface-muted);
  color: var(--text-primary);
  width: 38px;
  height: 38px;
  border-radius: 12px;
  font-size: 22px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 700;
  color: var(--text-primary);
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 12px 14px;
  background: var(--surface-muted);
  outline: none;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  border-color: rgba(47, 133, 90, 0.5);
  box-shadow: 0 0 0 4px rgba(47, 133, 90, 0.12);
}

.checkbox-group {
  justify-content: flex-end;
}

.checkbox-row {
  height: 100%;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 12px 14px;
  border-radius: 14px;
  background: var(--brand-soft);
  color: var(--brand-strong);
}

.label-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
}

.text-button {
  border: none;
  background: none;
  color: var(--accent);
  font-weight: 700;
  padding: 0;
}

.required {
  color: var(--danger);
}

.inline-category-box {
  padding: 18px;
  border-radius: 18px;
  background: #f3faf8;
  border: 1px solid #d6efe3;
}

.inline-actions,
.footer-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 12px 16px;
  font-weight: 700;
}

.primary-btn {
  background: linear-gradient(135deg, #48bb78 0%, #2f855a 100%);
  color: white;
  box-shadow: 0 12px 24px rgba(72, 187, 120, 0.24);
}

.secondary-btn {
  background: var(--surface-muted);
  color: var(--text-primary);
}

.error-text {
  color: var(--danger);
  background: var(--danger-soft);
  border: 1px solid #fed7aa;
  border-radius: 14px;
  padding: 12px 14px;
}

@media (max-width: 720px) {
  .dialog-card {
    padding: 20px;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .footer-actions {
    flex-direction: column-reverse;
  }

  .primary-btn,
  .secondary-btn {
    width: 100%;
  }
}
</style>
