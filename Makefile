SHELL := /bin/bash

# Convenience variables
UVX := uvx
BUMP := $(UVX) bump-my-version

.PHONY: test format format-check show-bump version bump-patch bump-minor bump-major

test:
	cd tests-dotnet && dotnet test

# Reformat C# to match .editorconfig style guide (Unity scripts + tests)
format:
	dotnet format tests-dotnet/BoardBlast.sln --no-restore
	@if [ -f Assembly-CSharp.csproj ]; then \
		dotnet format Assembly-CSharp.csproj --include Assets/Scripts --no-restore || true; \
	fi

# Verify C# is formatted (CI; exits non-zero if changes needed)
format-check:
	dotnet format tests-dotnet/BoardBlast.sln --verify-no-changes --no-restore
	@if [ -f Assembly-CSharp.csproj ]; then \
		dotnet format Assembly-CSharp.csproj --include Assets/Scripts --verify-no-changes --no-restore || true; \
	fi

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
