# Copyright 2026 dah4k
# SPDX-License-Identifier: EPL-2.0

DOCKER      ?= docker
_ANSI_NORM  := \033[0m
_ANSI_CYAN  := \033[36m

.PHONY: help usage
help usage:
	@grep -hE '^[0-9a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?##"}; {printf "$(_ANSI_CYAN)%-20s$(_ANSI_NORM) %s\n", $$1, $$2}'

.PHONY: all
all: ## Build .NET projects
	dotnet build

.PHONY: test
test: ## Run xUnit tests
	dotnet test
	dotnet run --project Cmd

.PHONY: fmt
fmt: ## Run fantomas formatter
	find . -type f -name "*.fs" -not -path "*obj*" | xargs dotnet fantomas

.PHONY: clean
clean: ## Remove build artefacts
	dotnet clean

.PHONY: distclean
distclean: ## Dist clean and prune devcontainers
	rm -rf */{bin,obj}
	$(DOCKER) image prune --force
	$(DOCKER) system prune --force

-include $(wildcard docker/*.mk)
