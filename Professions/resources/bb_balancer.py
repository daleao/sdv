import re
import sys

# Pattern to match opening and closing tags
TAG_PATTERN = re.compile(r'\[(\/?)(\w+)(?:=[^\]]+)?\]')

def check_tag_balance(file_path):
    stack = []  # To hold (tag_name, line_number)
    unbalanced_messages = []

    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    for line_number, line in enumerate(lines, start=1):
        for match in TAG_PATTERN.finditer(line):
            is_closing = match.group(1) == '/'
            tag_name = match.group(2)

            if is_closing:
                # Check if there's a corresponding opening tag in the stack
                for i in reversed(range(len(stack))):
                    if stack[i][0] == tag_name:
                        del stack[i]
                        break
                else:
                    # No matching opening tag found
                    unbalanced_messages.append(
                        f"Unbalanced tag [/{tag_name}] in line {line_number} does not have a corresponding opening tag [{tag_name}]."
                    )
            else:
                # Opening tag
                stack.append((tag_name, line_number))

    # Remaining tags in the stack are unclosed
    for tag_name, line_number in stack:
        unbalanced_messages.append(
            f"Unbalanced tag [{tag_name}] in line {line_number} does not have a corresponding closing tag [/{tag_name}]."
        )

    if not unbalanced_messages:
        print("All tags are balanced.")
    else:
        for msg in unbalanced_messages:
            print(msg)


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python check_tags.py <file_path>")
        sys.exit(1)

    check_tag_balance(sys.argv[1])
