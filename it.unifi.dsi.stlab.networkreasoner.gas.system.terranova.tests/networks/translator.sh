
IF=input.dat
OF=output.org

gawk -f pressure-regulator-org-input-data-translator.awk $IF > $OF
