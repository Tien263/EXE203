// Vietnam Address Validation Logic
// API Source: https://provinces.open-api.vn/

const AddressValidation = {
    // API Endpoint (Using depth=3 to get full hierarchy at once or fetching on demand)
    // We will use fetching on demand to reduce initial load if possible, or just load everything.
    // For simplicity and stability, we fetch provinces first.
    apiBase: "https://provinces.open-api.vn/api/",

    init: function (cityInputId, districtInputId, wardInputId, citySelectId, districtSelectId, wardSelectId) {
        this.cityInput = document.getElementById(cityInputId);
        this.districtInput = document.getElementById(districtInputId);
        this.wardInput = document.getElementById(wardInputId);

        this.citySelect = document.getElementById(citySelectId);
        this.districtSelect = document.getElementById(districtSelectId);
        this.wardSelect = document.getElementById(wardSelectId);

        if (!this.citySelect || !this.districtSelect || !this.wardSelect) return;

        this.loadProvinces();
        this.bindEvents();
    },

    loadProvinces: function () {
        fetch(this.apiBase + "?depth=1")
            .then(response => response.json())
            .then(data => {
                this.populateDropdown(this.citySelect, data, this.cityInput.value);
                // Trigger change to load districts if city is already selected
                if (this.cityInput.value) {
                    // We need to find the code for the selected city name to load districts
                    const selectedCity = data.find(p => p.name === this.cityInput.value);
                    if (selectedCity) {
                        this.citySelect.value = selectedCity.code;
                        this.loadDistricts(selectedCity.code);
                    }
                }
            })
            .catch(error => console.error("Error loading provinces:", error));
    },

    loadDistricts: function (provinceCode) {
        fetch(this.apiBase + "p/" + provinceCode + "?depth=2")
            .then(response => response.json())
            .then(data => {
                this.populateDropdown(this.districtSelect, data.districts, this.districtInput.value);
                // Trigger change to load wards if district is already selected
                if (this.districtInput.value) {
                    const selectedDistrict = data.districts.find(d => d.name === this.districtInput.value);
                    if (selectedDistrict) {
                        this.districtSelect.value = selectedDistrict.code;
                        this.loadWards(selectedDistrict.code);
                    }
                }
            })
            .catch(error => console.error("Error loading districts:", error));
    },

    loadWards: function (districtCode) {
        fetch(this.apiBase + "d/" + districtCode + "?depth=2")
            .then(response => response.json())
            .then(data => {
                this.populateDropdown(this.wardSelect, data.wards, this.wardInput.value);
                if (this.wardInput.value) {
                    const selectedWard = data.wards.find(w => w.name === this.wardInput.value);
                    if (selectedWard) {
                        this.wardSelect.value = selectedWard.code;
                    }
                }
            })
            .catch(error => console.error("Error loading wards:", error));
    },

    populateDropdown: function (selectElement, data, selectedName) {
        // Clear options except first one
        selectElement.innerHTML = '<option value="">-- Chọn --</option>';

        data.forEach(item => {
            const option = document.createElement("option");
            option.value = item.code;
            option.text = item.name;
            // Note: We don't set selected here usually, we set it by value logic above
            selectElement.appendChild(option);
        });
    },

    bindEvents: function () {
        // City Change
        this.citySelect.addEventListener("change", (e) => {
            const selectedOption = e.target.options[e.target.selectedIndex];
            const provinceCode = e.target.value;

            // Set Hidden Input Value (Name)
            this.cityInput.value = provinceCode ? selectedOption.text : "";

            // Reset lower levels
            this.districtSelect.innerHTML = '<option value="">-- Chọn --</option>';
            this.districtInput.value = "";
            this.wardSelect.innerHTML = '<option value="">-- Chọn --</option>';
            this.wardInput.value = "";

            if (provinceCode) {
                this.loadDistricts(provinceCode);
            }
        });

        // District Change
        this.districtSelect.addEventListener("change", (e) => {
            const selectedOption = e.target.options[e.target.selectedIndex];
            const districtCode = e.target.value;

            // Set Hidden Input Value (Name)
            this.districtInput.value = districtCode ? selectedOption.text : "";

            // Reset lower levels
            this.wardSelect.innerHTML = '<option value="">-- Chọn --</option>';
            this.wardInput.value = "";

            if (districtCode) {
                this.loadWards(districtCode);
            }
        });

        // Ward Change
        this.wardSelect.addEventListener("change", (e) => {
            const selectedOption = e.target.options[e.target.selectedIndex];
            // Set Hidden Input Value (Name)
            this.wardInput.value = selectedOption.value ? selectedOption.text : "";
        });
    }
};
