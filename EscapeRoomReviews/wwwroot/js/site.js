// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
	const MIN_CHARS = 2;
	const DEBOUNCE_MS = 250;

	const initEscapeRoomAutocomplete = (wrapper) => {
		const input = wrapper.querySelector("[data-role='autocomplete-input']");
		const list = wrapper.querySelector("[data-role='autocomplete-list']");
		const hidden = wrapper.querySelector("[data-role='autocomplete-hidden']");
		const loading = wrapper.querySelector("[data-role='autocomplete-loading']");
		const empty = wrapper.querySelector("[data-role='autocomplete-empty']");
		const url = wrapper.dataset.autocompleteUrl;

		if (!input || !list || !hidden || !url) return;

		let debounceId = null;
		let activeIndex = -1;
		let items = [];
		let abortController = null;

		const setFeedback = (state) => {
			if (loading) {
				loading.classList.toggle("show", state === "loading");
			}
			if (empty) {
				empty.classList.toggle("show", state === "empty");
			}
		};

		const clearList = () => {
			list.innerHTML = "";
			list.classList.remove("show");
			items = [];
			activeIndex = -1;
			input.setAttribute("aria-expanded", "false");
			setFeedback("none");
		};

		const setActive = (index) => {
			const options = list.querySelectorAll(".list-group-item");
			options.forEach((option, i) => {
				option.classList.toggle("active", i === index);
			});
			activeIndex = index;
		};

		const selectItem = (item) => {
			hidden.value = item.id;
			input.value = item.name;
			clearList();
		};

		const render = (results) => {
			list.innerHTML = "";
			setFeedback("none");
			if (!results.length) {
				list.classList.remove("show");
				input.setAttribute("aria-expanded", "false");
				setFeedback("empty");
				return;
			}

			results.forEach((item, index) => {
				const button = document.createElement("button");
				button.type = "button";
				button.className = "list-group-item list-group-item-action";
				button.textContent = item.name;
				button.addEventListener("click", () => selectItem(item));
				list.appendChild(button);
			});

			list.classList.add("show");
			input.setAttribute("aria-expanded", "true");
			items = results;
			activeIndex = -1;
		};

		const fetchResults = async (query) => {
			if (abortController) {
				abortController.abort();
			}

			abortController = new AbortController();
			setFeedback("loading");

			try {
				const response = await fetch(`${url}?term=${encodeURIComponent(query)}`,
					{ signal: abortController.signal, headers: { "X-Requested-With": "XMLHttpRequest" } });
				if (!response.ok) {
					setFeedback("empty");
					list.classList.remove("show");
					return;
				}

				const data = await response.json();
				render(Array.isArray(data) ? data : []);
			} catch (error) {
				if (error.name !== "AbortError") {
					setFeedback("empty");
					list.classList.remove("show");
					input.setAttribute("aria-expanded", "false");
				}
			}
		};

		input.addEventListener("input", () => {
			hidden.value = "";
			const query = input.value.trim();
			setFeedback("none");

			if (debounceId) {
				window.clearTimeout(debounceId);
			}

			if (query.length < MIN_CHARS) {
				clearList();
				return;
			}

			debounceId = window.setTimeout(() => fetchResults(query), DEBOUNCE_MS);
		});

		input.addEventListener("keydown", (event) => {
			if (!items.length) return;

			if (event.key === "ArrowDown") {
				event.preventDefault();
				const nextIndex = activeIndex + 1 < items.length ? activeIndex + 1 : 0;
				setActive(nextIndex);
			} else if (event.key === "ArrowUp") {
				event.preventDefault();
				const nextIndex = activeIndex - 1 >= 0 ? activeIndex - 1 : items.length - 1;
				setActive(nextIndex);
			} else if (event.key === "Enter" && activeIndex >= 0) {
				event.preventDefault();
				selectItem(items[activeIndex]);
			} else if (event.key === "Escape") {
				clearList();
			}
		});

		input.addEventListener("blur", () => {
			if (window.jQuery && jQuery.validator && hidden) {
				jQuery(hidden).valid();
			}
		});

		document.addEventListener("click", (event) => {
			if (!wrapper.contains(event.target)) {
				clearList();
			}
		});
	};

	const initValidationOnBlur = () => {
		if (window.jQuery && jQuery.validator) {
			jQuery.validator.setDefaults({
				ignore: ":hidden:not([data-role='autocomplete-hidden'])",
				onfocusout: function (element) {
					this.element(element);
				}
			});
		}
	};

	document.addEventListener("DOMContentLoaded", () => {
		initValidationOnBlur();
		document.querySelectorAll("[data-ajax-autocomplete]")
			.forEach(initEscapeRoomAutocomplete);
	});
})();
