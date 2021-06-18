const utils = {
    selectElements (elems, color = `#ffd460`) {
        let selectElement = (elem) => {
            const originalBackground   = elem.style.backgroundColor
            elem.style.backgroundColor = color

            return () => elem.style.backgroundColor = originalBackground
        }
        const unselects = elems.map(e => selectElement(e))
        return {
            unselect: () => unselects.forEach(unselect => unselect())
        }
    }
} 
export default utils