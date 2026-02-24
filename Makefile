SHELL := /bin/bash

# Convenience variables
UVX := uvx
BUMP := $(UVX) bump-my-version

.PHONY: show-bump version bump-patch bump-minor bump-major

show-bump: ## Show current version and possible bumps
	$(BUMP) show-bump

version: ## Print current version only
	@$(BUMP) show-bump | head -n1 | awk '{print $$1}'

bump-patch: check-clean ## Bump patch version (commit + tag)
	$(BUMP) bump patch

bump-minor: check-clean ## Bump minor version (commit + tag)
	$(BUMP) bump minor

bump-major: check-clean ## Bump major version (commit + tag)
	$(BUMP) bump major
