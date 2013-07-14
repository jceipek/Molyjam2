import os

files = os.listdir("mj2/Assets/Code")

for f in files:
	if f.endswith(".meta"):
		f_stripped = f[:-5]
		if not f_stripped in files:
			print f, "should be removed."