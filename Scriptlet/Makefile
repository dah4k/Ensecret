# Copyright 2026 dah4k
# SPDX-License-Identifier: EPL-2.0

_ANSI_NORM  := \033[0m
_ANSI_CYAN  := \033[36m

.PHONY: help usage
help usage:
	@grep -hE '^[0-9a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?##"}; {printf "$(_ANSI_CYAN)%-20s$(_ANSI_NORM) %s\n", $$1, $$2}'

.PHONY: all
all: $(TAGS) ## dotnet build
	dotnet build

.PHONY: clean
clean: ## dotnet clean
	dotnet clean

.PHONY: distclean
distclean: clean ## make clean && rm -rf bin obj
	rm -rf bin obj
