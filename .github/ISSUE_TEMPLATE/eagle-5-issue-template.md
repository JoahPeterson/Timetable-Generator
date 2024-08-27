name: "New Issue"
description: "Template for creating a new issue with description, acceptance criteria, labels, and story points."
title: "Enter the title of the issue here"
labels: 
  - "Task"
  - "Bug"
  - "Story"
body:
  - type: textarea
    id: description
    attributes:
      label: "Description"
      description: "Provide a detailed description of the issue."
      placeholder: "Describe the issue here..."

  - type: textarea
    id: acceptance_criteria
    attributes:
      label: "Acceptance Criteria (AC)"
      description: "List the conditions that need to be met for this issue to be considered complete."
      placeholder: "- [ ] Criterion 1\n- [ ] Criterion 2\n- [ ] Criterion 3"

  - type: dropdown
    id: issue_type
    attributes:
      label: "Issue Type"
      description: "Select the type of issue."
      options:
        - "Task"
        - "Bug"
        - "Story"

  - type: input
    id: story_points
    attributes:
      label: "Story Points Estimate"
      description: "Provide an estimate of story points for this issue."
      placeholder: "Enter story points (e.g., 1, 2, 3, 5, 8)"
      default: 1
      validations:
        required: true
        regex: "^[0-9]+$"
