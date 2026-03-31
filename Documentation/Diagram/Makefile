# Copyright 2026 dah4k
# SPDX-License-Identifier: EPL-2.0

SOURCE_PLANTUML   := $(wildcard *.puml)
DIAGRAM_SVG       := $(subst .puml,.svg,$(SOURCE_PLANTUML))
HOST_INSTALL      := $(shell which apt 2>/dev/null || which dnf 2>/dev/null || which zypper 2>/dev/null) install
HOST_UNINSTALL    := $(shell (which apt 2>/dev/null && echo "autoremove") || (which dnf 2>/dev/null && echo "autoremove") || (which zypper 2>/dev/null && echo "rm --clean-deps"))
HOST_REQUIREMENTS := plantuml

_ANSI_NORM := \033[0m
_ANSI_CYAN := \033[36m

.PHONY: help usage
help usage:
	@grep -hE '^[0-9a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?##"}; {printf "$(_ANSI_CYAN)%-24s$(_ANSI_NORM) %s\n", $$1, $$2}'

.PHONY: all
all: $(DIAGRAM_SVG) ## Generate diagrams

%.svg: %.puml
	plantuml -tsvg -theme blueprint $<

.PHONY: test
test: $(DIAGRAM_SVG) ## View generated diagrams
	qiv $^

.PHONY: clean
clean: ## Delete generated diagrams
	rm -f $(DIAGRAM_SVG)

.PHONY: install_requirements
install_requirements: ## Install host requirements
	sudo $(HOST_INSTALL) $(HOST_REQUIREMENTS)

.PHONY: uninstall_requirements
uninstall_requirements: ## Uninstall host requirements
	sudo $(HOST_UNINSTALL) $(HOST_REQUIREMENTS)
