.PHONY: test format format-check
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
