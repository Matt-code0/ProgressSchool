document.addEventListener("DOMContentLoaded", () => {
    const addExerciseBtn = document.getElementById("add-exercise-btn");
    const exercisesContainer = document.getElementById("exercises-container");

    if (!addExerciseBtn || !exercisesContainer) return;

    addExerciseBtn.addEventListener("click", () => {
        const index = exercisesContainer.children.length;

        const block = document.createElement("div");
        block.classList.add("exercise-block");
        block.innerHTML = `
            <div class="exercise-header">
                <input name="Exercises[${index}].Name" class="form-control" placeholder="Exercise name" />

                <span class="field-validation-error" data-valmsg-for="Exercises[${index}].Name"></span>

                <button type="button" class="btn btn-danger remove-exercise">X</button>
            </div>

            <div class="series-container"></div>
            <button type="button" class="btn btn-secondary add-series-btn">+ Add Set</button>
        `;

        exercisesContainer.appendChild(block);
        attachHandlers(block, index);
    });

    // attach "remove exercise" + add series handlers on existing blocks  
    document.querySelectorAll(".exercise-block").forEach((block, i) => {
        attachHandlers(block, i);
    });
});


function attachHandlers(exBlock, exerciseIndex) {
    // remove
    const removeBtn = exBlock.querySelector(".remove-exercise");
    if (removeBtn) {
        removeBtn.addEventListener("click", () => exBlock.remove());
    }

    // add series
    const addSeriesBtn = exBlock.querySelector(".add-series-btn");
    const seriesContainer = exBlock.querySelector(".series-container");

    if (addSeriesBtn && seriesContainer) {
        addSeriesBtn.addEventListener("click", () => {
            const sIndex = seriesContainer.children.length;

            const row = document.createElement("div");
            row.classList.add("series-row");
            row.innerHTML = `
                <span>Set ${sIndex + 1}</span>
                <input type="number" name="Exercises[${exerciseIndex}].Series[${sIndex}].Weight" placeholder="kg" class="form-control set-input" />
                <input type="number" name="Exercises[${exerciseIndex}].Series[${sIndex}].Reps" placeholder="reps" class="form-control set-input" />
                <button type="button" class="btn btn-danger remove-set">X</button>
            `;

            seriesContainer.appendChild(row);

            row.querySelector(".remove-set").addEventListener("click", () => row.remove());
        });
    }
}
